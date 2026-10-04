using System;
using System.IO;
using AgOpenGPS.IO;
using NUnit.Framework;

namespace AgOpenGPS.Tests.IO
{
    public class LegacyTrackFilesTests
    {
        private string fieldsDir;

        [SetUp]
        public void SetUp()
        {
            fieldsDir = Path.Combine(Path.GetTempPath(), "AgOpenGPS_LegacyTracks_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(fieldsDir);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(fieldsDir, true);
        }

        private string MakeField(string name)
        {
            var dir = Path.Combine(fieldsDir, name);
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "Field.txt"), "2021-May-01 10:00:00 AM\n$FieldDir\n" + name + "\n$Offsets\n0,0\nConvergence\n0\nStartFix\n47.5,19.0\n");
            return dir;
        }

        [Test]
        public void ConvertsV5ABAndCurveLines()
        {
            var dir = MakeField("Old");
            // Exactly as v5 FileSaveABLines / FileSaveCurveLines wrote them
            File.WriteAllText(Path.Combine(dir, "ABLines.txt"), "North,90,10.5,-20.25\nAB Second,0,0,0\n");
            File.WriteAllText(Path.Combine(dir, "CurveLines.txt"),
                "$CurveLines\nBend\n1.2\n3\n0,0,1.1\n1,1,1.2\n2,3,1.3\nBoundary Curve\n0.5\n2\n5,5,0.4\n6,6,0.5\nTooShort\n0\n1\n9,9,0\n");

            Assert.That(LegacyTrackFiles.ConvertAllFields(fieldsDir), Is.EqualTo(1));

            var tracks = TrackFiles.Load(dir);
            Assert.That(tracks.Count, Is.EqualTo(4));

            Assert.That(tracks[0].name, Is.EqualTo("AB North"));
            Assert.That(tracks[0].mode, Is.EqualTo(TrackMode.AB));
            Assert.That(tracks[0].heading, Is.EqualTo(Math.PI / 2).Within(1e-9));
            Assert.That(tracks[0].ptA.easting, Is.EqualTo(10.5));
            Assert.That(tracks[0].ptA.northing, Is.EqualTo(-20.25));
            Assert.That(tracks[0].ptB.easting, Is.EqualTo(110.5).Within(1e-3));
            Assert.That(tracks[1].name, Is.EqualTo("AB Second"));

            Assert.That(tracks[2].name, Is.EqualTo("Cu Bend"));
            Assert.That(tracks[2].mode, Is.EqualTo(TrackMode.Curve));
            Assert.That(tracks[2].heading, Is.EqualTo(1.2));
            Assert.That(tracks[2].curvePts.Count, Is.EqualTo(3));
            Assert.That(tracks[2].ptA.easting, Is.EqualTo(0));
            Assert.That(tracks[2].ptB.northing, Is.EqualTo(3));

            Assert.That(tracks[3].name, Is.EqualTo("Boundary Curve"));
            Assert.That(tracks[3].mode, Is.EqualTo(TrackMode.bndCurve));

            // Old files stay for older versions
            Assert.That(File.Exists(Path.Combine(dir, "ABLines.txt")), Is.True);
        }

        [Test]
        public void SkipsFieldsThatAlreadyHaveTrackLines()
        {
            var dir = MakeField("New");
            File.WriteAllText(Path.Combine(dir, "ABLines.txt"), "North,90,0,0\n");
            File.WriteAllText(Path.Combine(dir, "TrackLines.txt"), "$TrackLines\n");

            Assert.That(LegacyTrackFiles.ConvertAllFields(fieldsDir), Is.EqualTo(0));
            Assert.That(TrackFiles.Load(dir), Is.Empty);
        }

        [Test]
        public void CorruptFieldIsSkippedAndLeftUntouched()
        {
            var bad = MakeField("Bad");
            File.WriteAllText(Path.Combine(bad, "CurveLines.txt"), "$CurveLines\nBend\nnot a number\n");
            var good = MakeField("Good");
            File.WriteAllText(Path.Combine(good, "ABLines.txt"), "North,90,0,0\n");

            Assert.That(LegacyTrackFiles.ConvertAllFields(fieldsDir), Is.EqualTo(1));
            Assert.That(File.Exists(Path.Combine(bad, "TrackLines.txt")), Is.False);
            Assert.That(File.Exists(Path.Combine(good, "TrackLines.txt")), Is.True);
        }
    }
}
