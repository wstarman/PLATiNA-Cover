using System.Windows.Forms;
using System.Drawing;

namespace PLATiNA_Cover
{
    class CoveringStyle
    {
        public int x;
        public int y;
        public int w;
        public int h;

        public int r;
        public int g;
        public int b;
        public double a;

        public string imagePath;
        public bool keepAspectRatio;

        public static CoveringStyle GetCoveringStyle(Covering form)
        {
            var result = new CoveringStyle();
            result.x = form.Bounds.X;
            result.y = form.Bounds.Y;
            result.w = form.Bounds.Width;
            result.h = form.Bounds.Height;

            result.r = form.BackColor.R;
            result.g = form.BackColor.G;
            result.b = form.BackColor.B;
            result.a = form.Opacity;

            result.imagePath = form.imagePath;
            result.keepAspectRatio = form.keepAspectRatio;
            return result;
        }

        public static void SetCoveringFromStyle(Covering form, CoveringStyle style)
        {
            form.Bounds = new Rectangle(style.x, style.y, style.w, style.h);
            form.BackColor = Color.FromArgb(style.r, style.g, style.b);
            form.Opacity = style.a;
            form.imagePath = style.imagePath;
            form.keepAspectRatio = style.keepAspectRatio;
        }
    }
}
