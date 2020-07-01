﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dreary
{
    public static class DeviconServe
    {
        public static Image GetDevicon(string name) {
            switch (name)
            {
                case "GroupNode":
                    return new Bitmap("Devicons/folder.gif");
                case "CameraNode":
                    return new Bitmap("Devicons/icon_world_dynamic.gif");
                case "ActionList":
                    return new Bitmap("Devicons/icon_monitor_pc.gif");
                case "BillboardNode":
                    return new Bitmap("Devicons/note.gif");
                case "SkyboxNode":
                    return new Bitmap("Devicons/box.gif");
                default:
                    return new Bitmap("Devicons/icon_package.gif");
            }
        }
        public static ImageList GenImageList()
        {
            ImageList images = new ImageList();
            images.Images.Add(GetDevicon(""));
            images.Images.Add(GetDevicon("GroupNode"));
            images.Images.Add(GetDevicon("CameraNode"));
            images.Images.Add(GetDevicon("ActionList"));
            images.Images.Add(GetDevicon("BillboardNode"));
            images.Images.Add(GetDevicon("SkyboxNode"));
            return images;
        }
        public static int GetDeviconIndex(string name)
        {
            switch (name)
            {
                case "GroupNode":
                    return 1;
                case "CameraNode":
                    return 2;
                case "ActionList":
                    return 3;
                case "TextBillboardNode":
                case "BillboardNode":
                    return 4;
                case "SkyboxNode":
                    return 5;
                default:
                    return 0;
            }
        }
    }
}