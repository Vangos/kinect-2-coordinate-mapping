Kinect 2 Coordinate Mapping
=========================

This project demonstrates Kinect for Windows coordinate mapping using SDK version 2.0.

Learn how to draw the joints on screen and PERFECTLY align them with the color or depth frame.

Tutorial
---
Read a thorough tutorial to understand Kinect coordiante mapping: [http://pterneas.com/?p=1554] (http://pterneas.com/?p=1554)

Example
---

    foreach (Joint joint in body.Joints)
    {
        // 3D coordinates in meters
        CameraSpacePoint skeletonPoint = joint.Position;

        // 2D coordinates in pixels
        Point point = new Point();

        if (_mode == CameraMode.Color)
        {
            // Skeleton-to-Color mapping
            ColorSpacePoint colorPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(skeletonPoint);

            point.X = colorPoint.X;
            point.Y = colorPoint.Y;
        }
        else if (_mode == CameraMode.Depth)
        {
            // Skeleton-to-Depth mapping
            DepthSpacePoint depthPoint = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(skeletonPoint);

            point.X = depthPoint.X;
            point.Y = depthPoint.Y;
        }
    }

Credits
---
Developed by [Vangos Pterneas](http://pterneas.com) for [LightBuzz](http://lightbuzz.com)

License
---
You are free to use these libraries in personal and commercial projects by attributing the original creator of Vitruvius. Licensed under [MIT License](https://github.com/Vangos/kinect-2-coordinate-mapping/blob/master/LICENSE).

Vitruvius
---
Remember to check my other project, [Vitruvius] (https://github.com/LightBuzz/Vitruvius). Vitruvius is an open-source Kinect library that will speed-up the development process of your projects and will save you a ton of time. Vitruvius supports WPF, WinForms and WinRT.
