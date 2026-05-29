using AgOpenGPS.Core.Drawing;
using AgOpenGPS.Core.DrawLib;
using AgOpenGPS.Core.Models;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace AgOpenGPS
{
    public class CABLine
    {
        private readonly ColorRgba newAbLineColor = new ColorRgba(0.95f, 0.70f, 0.50f);
        private readonly ColorRgba pointsTextGreen = new ColorRgba(0.2f, 0.950f, 0.20f);
        private readonly ColorRgba pointARed = new ColorRgba(0.95f, 0.0f, 0.0f);
        private readonly ColorRgba pointBCyan = new ColorRgba(0.0f, 0.90f, 0.95f);
        private readonly ColorRgba referenceLineRed = new ColorRgba(0.930f, 0.2f, 0.2f);
        private readonly ColorRgba shadowAreaGray = new ColorRgba(0.5f, 0.5f, 0.5f, 0.2f);
        private readonly ColorRgba shadowLinesGray = new ColorRgba(0.55f, 0.55f, 0.55f, 0.2f);
        private readonly ColorRgba currentAbLinePurple = new ColorRgba(0.95f, 0.20f, 0.950f);
        private readonly ColorRgba extraGuidelinesBlack = new ColorRgba(0.0f, 0.0f, 0.0f, 0.5f);
        private readonly ColorRgba extraGuidelinesGreen = new ColorRgba(0.19907f, 0.6f, 0.19750f, 0.6f);

        public double abHeading, abLength;

        public bool isABValid;

        //the current AB guidance line
        public vec3 currentLinePtA = new vec3(0.0, 0.0, 0.0);
        public vec3 currentLinePtB = new vec3(0.0, 1.0, 0.0);

        public double distanceFromCurrentLinePivot;
        public double distanceFromRefLine;

        //pure pursuit values
        public vec2 goalPointAB = new vec2(0, 0);

        public int howManyPathsAway, lastHowManyPathsAway;
        public bool isMakingABLine;
        public bool isHeadingSameWay = true, lastIsHeadingSameWay;

        //public bool isOnTramLine;
        //public int tramBasedOn;
        public double ppRadiusAB;

        public vec2 radiusPointAB = new vec2(0, 0);
        public double rEastAB, rNorthAB;

        public double snapDistance, lastSecond = 0;
        public double steerAngleAB;
        public int lineWidth, numGuideLines;

        //design
        public vec2 desPtA = new vec2(0.2, 0.15);
        public vec2 desPtB = new vec2(0.3, 0.3);

        public vec2 desLineEndA = new vec2(0.0, 0.0);
        public vec2 desLineEndB = new vec2(999997, 1.0);

        public double desHeading = 0;

        public string desName = "";

        //autosteer errors
        public double pivotDistanceError, pivotDistanceErrorLast, pivotDerivative;

        //derivative counters
        private int counter2;

        public double inty;
        public double pivotErrorTotal;

        // Tree planting: signed distance in meters from nearest perpendicular tramline
        public double treePlantDistance;
        // Tree planting: smoothed distance for display
        public double avgTreePlantDistance;
        // Tree planting: center point on the nearest tramline (for drawing target circles)
        public vec2 treePlantTargetPoint = new vec2(0, 0);

        //Color tramColor = Color.YellowGreen;

        //pointers to mainform controls
        private readonly FormGPS mf;

        public CABLine(FormGPS _f)
        {
            //constructor
            mf = _f;
            //isOnTramLine = true;
            lineWidth = Properties.Settings.Default.setDisplay_lineWidth;
            abLength = 2000;
            numGuideLines = Properties.Settings.Default.setAS_numGuideLines;
        }

        public void BuildCurrentABLineList(vec3 pivot)
        {
            if (mf.trk.gArr.Count < mf.trk.idx || mf.trk.idx < 0) return;

            CTrk track = mf.trk.gArr[mf.trk.idx];

            if (!isABValid || ((mf.secondsSinceStart - lastSecond) > 0.66 && (!mf.isBtnAutoSteerOn || mf.mc.steerSwitchHigh)))
            {
                lastSecond = mf.secondsSinceStart;

                double dx, dy;

                abHeading = track.heading;

                track.endPtA.easting = track.ptA.easting - (Math.Sin(abHeading) * abLength);
                track.endPtA.northing = track.ptA.northing - (Math.Cos(abHeading) * abLength);

                track.endPtB.easting = track.ptB.easting + (Math.Sin(abHeading) * abLength);
                track.endPtB.northing = track.ptB.northing + (Math.Cos(abHeading) * abLength);

                //move the ABLine over based on the overlap amount set in
                double widthMinusOverlap = mf.tool.width - mf.tool.overlap;

                //x2-x1
                dx = track.endPtB.easting - track.endPtA.easting;
                //z2-z1
                dy = track.endPtB.northing - track.endPtA.northing;

                distanceFromRefLine = ((dy * mf.guidanceLookPos.easting) - (dx * mf.guidanceLookPos.northing) + (track.endPtB.easting
                                        * track.endPtA.northing) - (track.endPtB.northing * track.endPtA.easting))
                                            / Math.Sqrt((dy * dy) + (dx * dx));

                distanceFromRefLine -= (0.5 * widthMinusOverlap);

                isHeadingSameWay = Math.PI - Math.Abs(Math.Abs(pivot.heading - abHeading) - Math.PI) < glm.PIBy2;

                //if (mf.yt.isYouTurnTriggered && !mf.yt.isGoingStraightThrough) isHeadingSameWay = !isHeadingSameWay;

                //Which ABLine is the vehicle on, negative is left and positive is right side

                double RefDist = (distanceFromRefLine + (isHeadingSameWay ? mf.tool.offset : -mf.tool.offset) - track.nudgeDistance) / widthMinusOverlap;

                if (RefDist < 0) howManyPathsAway = (int)(RefDist - 0.5);
                else howManyPathsAway = (int)(RefDist + 0.5);
            }

            if (!isABValid || howManyPathsAway != lastHowManyPathsAway || (isHeadingSameWay != lastIsHeadingSameWay && mf.tool.offset != 0))
            {
                isABValid = true;
                lastHowManyPathsAway = howManyPathsAway;
                lastIsHeadingSameWay = isHeadingSameWay;

                double widthMinusOverlap = mf.tool.width - mf.tool.overlap;

                double distAway = widthMinusOverlap * howManyPathsAway + (isHeadingSameWay ? -mf.tool.offset : mf.tool.offset) + track.nudgeDistance;

                distAway += (0.5 * widthMinusOverlap);

                //move the curline as well. 
                vec2 nudgePtA = new vec2(track.ptA);
                vec2 nudgePtB = new vec2(track.ptB);

                //depending which way you are going, the offset can be either side
                vec2 point1 = new vec2((Math.Cos(-abHeading) * distAway) + nudgePtA.easting, (Math.Sin(-abHeading) * distAway) + nudgePtA.northing);

                vec2 point2 = new vec2((Math.Cos(-abHeading) * distAway) + nudgePtB.easting, (Math.Sin(-abHeading) * distAway) + nudgePtB.northing);

                //create the new line extent points for current ABLine based on original heading of AB line
                currentLinePtA.easting = point1.easting - (Math.Sin(abHeading) * abLength);
                currentLinePtA.northing = point1.northing - (Math.Cos(abHeading) * abLength);

                currentLinePtB.easting = point2.easting + (Math.Sin(abHeading) * abLength);
                currentLinePtB.northing = point2.northing + (Math.Cos(abHeading) * abLength);

                currentLinePtA.heading = abHeading;
                currentLinePtB.heading = abHeading;
            }
        }

        public void GetCurrentABLine(vec3 pivot, vec3 steer)
        {
            double dx, dy;

            //Check uturn first
            if (mf.yt.isYouTurnTriggered && mf.yt.DistanceFromYouTurnLine())//do the pure pursuit from youTurn
            {
                //now substitute what it thinks are AB line values with auto turn values
                steerAngleAB = mf.yt.steerAngleYT;
                distanceFromCurrentLinePivot = mf.yt.distanceFromCurrentLine;

                goalPointAB = mf.yt.goalPointYT;
                radiusPointAB.easting = mf.yt.radiusPointYT.easting;
                radiusPointAB.northing = mf.yt.radiusPointYT.northing;
                ppRadiusAB = mf.yt.ppRadiusYT;

                mf.vehicle.modeTimeCounter = 0;
                mf.vehicle.modeActualXTE = (distanceFromCurrentLinePivot);
            }

            //Stanley
            else if (mf.isStanleyUsed)
                mf.gyd.StanleyGuidanceABLine(currentLinePtA, currentLinePtB, pivot, steer);

            //Pure Pursuit
            else
            {
                //get the distance from currently active AB line
                //x2-x1
                dx = currentLinePtB.easting - currentLinePtA.easting;
                //z2-z1
                dy = currentLinePtB.northing - currentLinePtA.northing;

                //how far from current AB Line is fix
                distanceFromCurrentLinePivot = ((dy * pivot.easting) - (dx * pivot.northing) + (currentLinePtB.easting
                            * currentLinePtA.northing) - (currentLinePtB.northing * currentLinePtA.easting))
                            / Math.Sqrt((dy * dy) + (dx * dx));

                //integral slider is set to 0
                if (mf.vehicle.purePursuitIntegralGain != 0 && !mf.isReverse)
                {
                    pivotDistanceError = distanceFromCurrentLinePivot * 0.2 + pivotDistanceError * 0.8;

                    if (counter2++ > 4)
                    {
                        pivotDerivative = pivotDistanceError - pivotDistanceErrorLast;
                        pivotDistanceErrorLast = pivotDistanceError;
                        counter2 = 0;
                        pivotDerivative *= 2;

                        //limit the derivative
                        //if (pivotDerivative > 0.03) pivotDerivative = 0.03;
                        //if (pivotDerivative < -0.03) pivotDerivative = -0.03;
                        //if (Math.Abs(pivotDerivative) < 0.01) pivotDerivative = 0;
                    }

                    //pivotErrorTotal = pivotDistanceError + pivotDerivative;

                    if (mf.isBtnAutoSteerOn
                        && Math.Abs(pivotDerivative) < (0.1)
                        && mf.avgSpeed > 2.5
                        && !mf.yt.isYouTurnTriggered)
                    //&& Math.Abs(pivotDistanceError) < 0.2)

                    {
                        //if over the line heading wrong way, rapidly decrease integral
                        if ((inty < 0 && distanceFromCurrentLinePivot < 0) || (inty > 0 && distanceFromCurrentLinePivot > 0))
                        {
                            inty += pivotDistanceError * mf.vehicle.purePursuitIntegralGain * -0.04;
                        }
                        else
                        {
                            if (Math.Abs(distanceFromCurrentLinePivot) > 0.02)
                            {
                                inty += pivotDistanceError * mf.vehicle.purePursuitIntegralGain * -0.02;
                                if (inty > 0.2) inty = 0.2;
                                else if (inty < -0.2) inty = -0.2;
                            }
                        }
                    }
                    else inty *= 0.95;
                }
                else inty = 0;

                // ** Pure pursuit ** - calc point on ABLine closest to current position
                double U = (((pivot.easting - currentLinePtA.easting) * dx)
                            + ((pivot.northing - currentLinePtA.northing) * dy))
                            / ((dx * dx) + (dy * dy));

                //point on AB line closest to pivot axle point
                rEastAB = currentLinePtA.easting + (U * dx);
                rNorthAB = currentLinePtA.northing + (U * dy);

                //update base on autosteer settings and distance from line
                double goalPointDistance = mf.vehicle.UpdateGoalPointDistance();

                if (mf.isReverse ^ isHeadingSameWay)
                {
                    goalPointAB.easting = rEastAB + (Math.Sin(abHeading) * goalPointDistance);
                    goalPointAB.northing = rNorthAB + (Math.Cos(abHeading) * goalPointDistance);
                }
                else
                {
                    goalPointAB.easting = rEastAB - (Math.Sin(abHeading) * goalPointDistance);
                    goalPointAB.northing = rNorthAB - (Math.Cos(abHeading) * goalPointDistance);
                }

                //calc "D" the distance from pivot axle to lookahead point
                double goalPointDistanceDSquared
                    = glm.DistanceSquared(goalPointAB.northing, goalPointAB.easting, pivot.northing, pivot.easting);

                //calculate the the new x in local coordinates and steering angle degrees based on wheelbase
                double localHeading;

                if (isHeadingSameWay) localHeading = glm.twoPI - mf.fixHeading + inty;
                else localHeading = glm.twoPI - mf.fixHeading - inty;

                ppRadiusAB = goalPointDistanceDSquared / (2 * (((goalPointAB.easting - pivot.easting) * Math.Cos(localHeading))
                    + ((goalPointAB.northing - pivot.northing) * Math.Sin(localHeading))));

                steerAngleAB = glm.toDegrees(Math.Atan(2 * (((goalPointAB.easting - pivot.easting) * Math.Cos(localHeading))
                    + ((goalPointAB.northing - pivot.northing) * Math.Sin(localHeading))) * mf.vehicle.VehicleConfig.Wheelbase
                    / goalPointDistanceDSquared));

                if (mf.ahrs.imuRoll != 88888)
                    steerAngleAB += mf.ahrs.imuRoll * -mf.gyd.sideHillCompFactor;

                //steerAngleAB *= 1.4;

                if (steerAngleAB < -mf.vehicle.maxSteerAngle) steerAngleAB = -mf.vehicle.maxSteerAngle;
                if (steerAngleAB > mf.vehicle.maxSteerAngle) steerAngleAB = mf.vehicle.maxSteerAngle;

                //limit circle size for display purpose
                if (ppRadiusAB < -500) ppRadiusAB = -500;
                if (ppRadiusAB > 500) ppRadiusAB = 500;

                radiusPointAB.easting = pivot.easting + (ppRadiusAB * Math.Cos(localHeading));
                radiusPointAB.northing = pivot.northing + (ppRadiusAB * Math.Sin(localHeading));

                //if (mf.isConstantContourOn)
                //{
                //    //angular velocity in rads/sec  = 2PI * m/sec * radians/meters

                //    //clamp the steering angle to not exceed safe angular velocity
                //    if (Math.Abs(mf.setAngVel) > 1000)
                //    {
                //        //mf.setAngVel = mf.setAngVel < 0 ? -mf.vehicle.maxAngularVelocity : mf.vehicle.maxAngularVelocity;
                //        mf.setAngVel = mf.setAngVel < 0 ? -1000 : 1000;
                //    }
                //}

                //distance is negative if on left, positive if on right
                if (!isHeadingSameWay)
                    distanceFromCurrentLinePivot *= -1.0;

                //used for acquire/hold mode
                mf.vehicle.modeActualXTE = (distanceFromCurrentLinePivot);

                double steerHeadingError = (pivot.heading - abHeading);
                //Fix the circular error
                if (steerHeadingError > Math.PI)
                    steerHeadingError -= Math.PI;
                else if (steerHeadingError < -Math.PI)
                    steerHeadingError += Math.PI;

                if (steerHeadingError > glm.PIBy2)
                    steerHeadingError -= Math.PI;
                else if (steerHeadingError < -glm.PIBy2)
                    steerHeadingError += Math.PI;

                mf.vehicle.modeActualHeadingError = glm.toDegrees(steerHeadingError);

                //Convert to millimeters
                mf.guidanceLineDistanceOff = (short)Math.Round(distanceFromCurrentLinePivot * 1000.0, MidpointRounding.AwayFromZero);
                mf.guidanceLineSteerAngle = (short)(steerAngleAB * 100);
            }

            //mf.setAngVel = 0.277777 * mf.avgSpeed * (Math.Tan(glm.toRadians(steerAngleAB))) / mf.vehicle.wheelbase;
            //mf.setAngVel = glm.toDegrees(mf.setAngVel);
        }

        public void DrawABLineNew()
        {
            //ABLine currently being designed
            GeoCoord[] desLineEndPoints = { desLineEndA.ToGeoCoord(), desLineEndB.ToGeoCoord() };

            GLW.SetLineWidth(lineWidth);
            GLW.SetColor(newAbLineColor);
            GLW.DrawLinesPrimitive(desLineEndPoints);

            GLW.SetColor(pointsTextGreen);
            mf.font.DrawText3D(desPtA.easting, desPtA.northing, "&A", mf.camHeading);
            mf.font.DrawText3D(desPtB.easting, desPtB.northing, "&B", mf.camHeading);
        }

        public void DrawABLines()
        {
            // Don't draw if AB line is not valid yet (prevents drawing with uninitialized values after track switch)
            if (!isABValid) return;

            // Draw AB Points
            CTrk track = mf.trk.gArr[mf.trk.idx];
            GLW.SetPointSize(8.0f);
            GLW.BeginPointsPrimitive();

            GLW.SetColor(pointBCyan);
            GLW.Vertex2(track.ptB.ToGeoCoord());
            GLW.SetColor(pointARed);
            GLW.Vertex2(track.ptA.ToGeoCoord());
            GLW.EndPrimitive();

            GLW.DrawPoint(track.ptA.ToGeoCoord());

            if (!isMakingABLine)
            {
                mf.font.DrawText3D(track.ptA.easting, track.ptA.northing, "&A", mf.camHeading);
                mf.font.DrawText3D(track.ptB.easting, track.ptB.northing, "&B", mf.camHeading);
            }

            GLW.SetPointSize(1.0f);

            //Draw reference AB line
            GeoCoord[] abEndPoints = { track.endPtA.ToGeoCoord(), track.endPtB.ToGeoCoord() };
            GLW.SetLineWidth(4.0f);
            GLW.EnableLineStipple();
            GLW.SetLineStipple(1, 0x0F00);
            GLW.SetColor(referenceLineRed);
            GLW.DrawLinesPrimitive(abEndPoints);
            GLW.DisableLineStipple();

            // shadow
            double shadowOffset = isHeadingSameWay ? mf.tool.offset : -mf.tool.offset;
            GeoCoord ptA = currentLinePtA.ToGeoCoord();
            GeoCoord ptB = currentLinePtB.ToGeoCoord();
            GeoDir abDir = new GeoDir(abHeading);
            GeoDir perpendicalurRightDir = abDir.PerpendicularRight;
            GeoDelta rightOffset = (shadowOffset + 0.5 * mf.tool.width) * perpendicalurRightDir;
            GeoDelta leftOffset = (shadowOffset - 0.5 * mf.tool.width) * perpendicalurRightDir;

            GeoCoord[] shadowCoords = {
                ptA + leftOffset,
                ptA + rightOffset,
                ptB + rightOffset,
                ptB + leftOffset
            };

            GLW.SetColor(shadowAreaGray);
            GLW.DrawTriangleFanPrimitive(shadowCoords);
            GLW.SetColor(shadowLinesGray);
            GLW.SetLineWidth(1.0f);
            GLW.DrawLineLoopPrimitive(shadowCoords);

            //draw current AB Line
            GeoCoord[] currentAbLine = { currentLinePtA.ToGeoCoord(), currentLinePtB.ToGeoCoord() };
            LineStyle blackBackgroundStyle = new LineStyle(lineWidth * 3, Colors.Black);
            LineStyle purpleForgroundStyle = new LineStyle(lineWidth, currentAbLinePurple);
            GLW.DrawLinesPrimitiveLayered(
                currentAbLine,
                blackBackgroundStyle,
                purpleForgroundStyle);

            if (mf.isSideGuideLines && mf.camera.camSetDistance > mf.tool.width * -400)
            {
                double toolWidth = mf.tool.width - mf.tool.overlap;
                GeoLineSegment currentLine = new GeoLineSegment(currentLinePtA.ToGeoCoord(), currentLinePtB.ToGeoCoord());
                GeoDir perpendicularRightDir = currentLine.Direction.PerpendicularRight;
                GeoLineSegment[] lines = new GeoLineSegment[2 * numGuideLines];
                int linesIndex = 0;

                double oddOffset = 2 * (isHeadingSameWay ? mf.tool.offset : -mf.tool.offset);
                for (int i = 1; i <= numGuideLines; i += 2)
                {
                    GeoLineSegment rightOddLine = currentLine.Shifted((toolWidth * i + oddOffset) * perpendicularRightDir);
                    GeoLineSegment leftOddLine = currentLine.Shifted((toolWidth * -i + oddOffset) * perpendicularRightDir);
                    lines[linesIndex++] = rightOddLine;
                    lines[linesIndex++] = leftOddLine;
                }
                for (int i = 2; i <= numGuideLines; i += 2)
                {
                    GeoLineSegment rightEvenLine = currentLine.Shifted((toolWidth * i) * perpendicularRightDir);
                    GeoLineSegment leftEvenLine = currentLine.Shifted((toolWidth * -i) * perpendicularRightDir);
                    lines[linesIndex++] = rightEvenLine;
                    lines[linesIndex++] = leftEvenLine;
                }
                LineStyle extraGuidelinesBackgroundStyle = new LineStyle(lineWidth * 3, extraGuidelinesBlack);
                LineStyle extraGuidelinesForegroundStyle = new LineStyle(lineWidth, extraGuidelinesGreen);
                GLW.DrawLinesPrimitiveLayered(
                    lines,
                    extraGuidelinesBackgroundStyle,
                    extraGuidelinesForegroundStyle);
            }
            mf.yt.DrawYouTurn();

            GLW.SetPointSize(1.0f);
            GLW.SetLineWidth(1.0f);
        }

        public void BuildTram()
        {
            if (mf.tram.generateMode != 1)
            {
                mf.tram.BuildTramBnd();
            }
            else
            {
                mf.tram.tramBndOuterArr?.Clear();
                mf.tram.tramBndInnerArr?.Clear();
            }

            mf.tram.tramList?.Clear();
            mf.tram.tramArr?.Clear();

            if (mf.tram.generateMode == 2) return;

            List<vec2> tramRef = new List<vec2>();

            bool isBndExist = mf.bnd.bndList.Count != 0;

            abHeading = mf.trk.gArr[mf.trk.idx].heading;

            double hsin = Math.Sin(abHeading);
            double hcos = Math.Cos(abHeading);

            double len = glm.Distance(mf.trk.gArr[mf.trk.idx].endPtA, mf.trk.gArr[mf.trk.idx].endPtB);
            //divide up the AB line into segments
            vec2 P1 = new vec2();
            for (int i = 0; i < (int)len; i += 4)
            {
                P1.easting = (hsin * i) + mf.trk.gArr[mf.trk.idx].endPtA.easting;
                P1.northing = (hcos * i) + mf.trk.gArr[mf.trk.idx].endPtA.northing;
                tramRef.Add(P1);
            }

            //create list of list of points of triangle strip of AB Highlight
            double headingCalc = abHeading + glm.PIBy2;

            hsin = Math.Sin(headingCalc);
            hcos = Math.Cos(headingCalc);

            mf.tram.tramList?.Clear();
            mf.tram.tramArr?.Clear();

            //no boundary starts on first pass
            int cntr = 0;
            if (isBndExist)
            {
                if (mf.tram.generateMode == 1)
                    cntr = 0;
                else
                    cntr = 1;
            }

            double widd;
            for (int i = cntr; i < mf.tram.passes; i++)
            {
                mf.tram.tramArr = new List<vec2>
                {
                    Capacity = 128
                };

                mf.tram.tramList.Add(mf.tram.tramArr);

                widd = (mf.tram.tramWidth * 0.5) - mf.tram.halfWheelTrack;
                widd += (mf.tram.tramWidth * i);

                for (int j = 0; j < tramRef.Count; j++)
                {
                    P1.easting = hsin * widd + tramRef[j].easting;
                    P1.northing = (hcos * widd) + tramRef[j].northing;

                    if (!isBndExist || mf.bnd.bndList[0].fenceLineEar.IsPointInPolygon(P1))
                    {
                        mf.tram.tramArr.Add(P1);
                    }
                }
            }

            for (int i = cntr; i < mf.tram.passes; i++)
            {
                mf.tram.tramArr = new List<vec2>
                {
                    Capacity = 128
                };

                mf.tram.tramList.Add(mf.tram.tramArr);

                widd = (mf.tram.tramWidth * 0.5) + mf.tram.halfWheelTrack;
                widd += (mf.tram.tramWidth * i);

                for (int j = 0; j < tramRef.Count; j++)
                {
                    P1.easting = (hsin * widd) + tramRef[j].easting;
                    P1.northing = (hcos * widd) + tramRef[j].northing;

                    if (!isBndExist || mf.bnd.bndList[0].fenceLineEar.IsPointInPolygon(P1))
                    {
                        mf.tram.tramArr.Add(P1);
                    }
                }
            }

            tramRef?.Clear();
            //outside tram

            if (mf.bnd.bndList.Count == 0 || mf.tram.passes != 0)
            {
                //return;
            }
        }

        public void CalculateTreePlantDistance()
        {
            if (mf.trk.gArr == null || mf.trk.gArr.Count == 0 || mf.trk.idx < 0) return;

            CTrk track = mf.trk.gArr[mf.trk.idx];
            if (track.mode != TrackMode.AB) return;

            // AB line direction vector
            double dx = track.endPtB.easting - track.endPtA.easting;
            double dy = track.endPtB.northing - track.endPtA.northing;
            double lineLen = Math.Sqrt(dx * dx + dy * dy);
            if (lineLen < 0.001) return;

            // Unit direction vector along AB line
            double ux = dx / lineLen;
            double uy = dy / lineLen;

            // Vector from ptA to current tool position (antenna is on the implement)
            double px = mf.toolPos.easting - track.endPtA.easting;
            double py = mf.toolPos.northing - track.endPtA.northing;

            // Get settings
            double gridSpacing = Properties.ToolSettings.Default.setTool_treePlantGridSpacing;
            if (gridSpacing < 0.1) gridSpacing = 0.1;
            double angleDeg = Properties.ToolSettings.Default.setTool_treePlantAngle;
            double angleRad = angleDeg * Math.PI / 180.0;

            // Tramline direction: rotate AB direction by angle
            double tDirX = ux * Math.Cos(angleRad) - uy * Math.Sin(angleRad);
            double tDirY = uy * Math.Cos(angleRad) + ux * Math.Sin(angleRad);

            // Cross-tramline direction (perpendicular to tramline, in the grid direction)
            double cDirX = -tDirY;
            double cDirY = tDirX;

            // Project position onto cross-tramline direction
            double crossCoord = px * cDirX + py * cDirY;

            // Modulo gridSpacing, snap to nearest tramline
            double dist = crossCoord % gridSpacing;
            if (dist < 0) dist += gridSpacing;

            double halfGrid = gridSpacing * 0.5;
            if (dist > halfGrid) dist -= gridSpacing;

            // Calculate the target point on the nearest tramline
            // Project position onto tramline direction to get along-tramline coordinate
            double alongTram = px * tDirX + py * tDirY;
            // Target = position - (perpendicular distance) * crossDirection
            treePlantTargetPoint.easting = mf.toolPos.easting - dist * cDirX;
            treePlantTargetPoint.northing = mf.toolPos.northing - dist * cDirY;

            // Sign convention: positive = behind (haven't reached it), negative = ahead
            if (!isHeadingSameWay) dist = -dist;

            treePlantDistance = dist;
        }

        public void DrawTreePlant()
        {
            if (!isABValid) return;
            if (mf.trk.gArr == null || mf.trk.idx < 0) return;
            if (mf.trk.gArr[mf.trk.idx].mode != TrackMode.AB) return;

            CTrk track = mf.trk.gArr[mf.trk.idx];
            double gridSpacing = Properties.ToolSettings.Default.setTool_treePlantGridSpacing;
            if (gridSpacing < 0.1) return;

            double dx = track.endPtB.easting - track.endPtA.easting;
            double dy = track.endPtB.northing - track.endPtA.northing;
            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len < 0.01) return;

            double ux = dx / len;
            double uy = dy / len;

            // Tramline direction from angle setting
            double angleDeg = Properties.ToolSettings.Default.setTool_treePlantAngle;
            double angleRad = angleDeg * Math.PI / 180.0;
            double tDirX = ux * Math.Cos(angleRad) - uy * Math.Sin(angleRad);
            double tDirY = uy * Math.Cos(angleRad) + ux * Math.Sin(angleRad);

            double sideExtension = Properties.ToolSettings.Default.setTool_treePlantSideExtension;

            // --- Part A: Draw tramlines at configured angle ---
            // Tramline crossing points along AB line are at intervals of gridSpacing / sin(angle)
            double sinAngle = Math.Sin(angleRad);
            if (Math.Abs(sinAngle) < 0.01) return; // Near-parallel tramlines, skip
            double spacingAlongAB = gridSpacing / sinAngle;

            double pivotAlong = ((mf.toolPos.easting - track.endPtA.easting) * dx
                               + (mf.toolPos.northing - track.endPtA.northing) * dy) / (len * len);

            int iStart = Math.Max(0, (int)Math.Floor((pivotAlong - 0.5) * len / spacingAlongAB) - 1);
            int iEnd = (int)Math.Ceiling((pivotAlong + 0.5) * len / spacingAlongAB) + 1;

            GL.LineWidth(2);
            GL.Color4(0.95f, 0.75f, 0.1f, 0.8f); // Orange-yellow
            GL.Begin(PrimitiveType.Lines);
            for (int i = iStart; i <= iEnd; i++)
            {
                double distAlong = i * spacingAlongAB;
                double cx = track.endPtA.easting + ux * distAlong;
                double cy = track.endPtA.northing + uy * distAlong;
                GL.Vertex3(cx - tDirX * sideExtension, cy - tDirY * sideExtension, 0);
                GL.Vertex3(cx + tDirX * sideExtension, cy + tDirY * sideExtension, 0);
            }
            GL.End();

            // --- Part B: Target circles (bullseye) ---
            double centerX = treePlantTargetPoint.easting;
            double centerY = treePlantTargetPoint.northing;

            // Inner circle: 5cm radius (1 degree threshold)
            // Outer circle: 25cm radius (5 degree threshold)
            double innerRadius = 0.05;
            double outerRadius = 0.25;
            const int segments = 36;

            double absDist = Math.Abs(treePlantDistance);

            // Outer circle - background (black halo)
            GL.LineWidth(5);
            GL.Color4(0, 0, 0, 0.8);
            GL.Begin(PrimitiveType.LineLoop);
            for (int i = 0; i < segments; i++)
            {
                double angle = i * Math.PI * 2.0 / segments;
                GL.Vertex3(centerX + outerRadius * Math.Cos(angle),
                           centerY + outerRadius * Math.Sin(angle), 0);
            }
            GL.End();

            // Outer circle - foreground (color based on distance)
            GL.LineWidth(2);
            if (absDist <= innerRadius)
                GL.Color4(0.1f, 0.95f, 0.1f, 0.9f); // Green: on target
            else if (absDist <= outerRadius)
                GL.Color4(0.95f, 0.95f, 0.1f, 0.9f); // Yellow: close
            else
                GL.Color4(0.95f, 0.2f, 0.1f, 0.9f); // Red: far
            GL.Begin(PrimitiveType.LineLoop);
            for (int i = 0; i < segments; i++)
            {
                double angle = i * Math.PI * 2.0 / segments;
                GL.Vertex3(centerX + outerRadius * Math.Cos(angle),
                           centerY + outerRadius * Math.Sin(angle), 0);
            }
            GL.End();

            // Inner circle - background
            GL.LineWidth(3);
            GL.Color4(0, 0, 0, 0.8);
            GL.Begin(PrimitiveType.LineLoop);
            for (int i = 0; i < segments; i++)
            {
                double angle = i * Math.PI * 2.0 / segments;
                GL.Vertex3(centerX + innerRadius * Math.Cos(angle),
                           centerY + innerRadius * Math.Sin(angle), 0);
            }
            GL.End();

            // Inner circle - foreground (green)
            GL.LineWidth(1);
            GL.Color4(0.1f, 0.95f, 0.1f, 0.9f);
            GL.Begin(PrimitiveType.LineLoop);
            for (int i = 0; i < segments; i++)
            {
                double angle = i * Math.PI * 2.0 / segments;
                GL.Vertex3(centerX + innerRadius * Math.Cos(angle),
                           centerY + innerRadius * Math.Sin(angle), 0);
            }
            GL.End();

            // Center dot
            GL.PointSize(6);
            GL.Color4(0.1f, 0.95f, 0.1f, 0.9f);
            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(centerX, centerY, 0);
            GL.End();

            // --- Part C: Direction line from antenna to target center ---
            GL.LineWidth(1);
            GL.Color4(0.95f, 0.95f, 0.1f, 0.6f); // Yellow dashed
            GL.Enable(EnableCap.LineStipple);
            GL.LineStipple(1, 0x0F0F);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(mf.toolPos.easting, mf.toolPos.northing, 0);
            GL.Vertex3(centerX, centerY, 0);
            GL.End();
            GL.Disable(EnableCap.LineStipple);

            GL.LineWidth(1);
            GL.PointSize(1);
        }
    }
}