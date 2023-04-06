using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Teststand_UI
{
    [ToolboxBitmap(typeof(CheckedListBox)), DesignerCategory("ATE")]
    public class CheckedListBoxWithBgImage : CheckedListBox
    {

        public CheckedListBoxWithBgImage() : base()
        {
            SetStyle(
                ControlStyles.Opaque |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            DrawMode = DrawMode.OwnerDrawFixed;


        }

        [DllImport("uxtheme", ExactSpelling = true)]
        private extern static int DrawThemeParentBackground(
            IntPtr hWnd,
            IntPtr hdc,
            ref Rectangle pRect
            );


        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var rec = ClientRectangle;

            IntPtr hdc = g.GetHdc();
            DrawThemeParentBackground(this.Handle, hdc, ref rec);
            g.ReleaseHdc(hdc);

            using (Region reg = new Region(e.ClipRectangle))
            {
                if (Items.Count > 0)
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        rec = GetItemRectangle(i);

                        if (e.ClipRectangle.IntersectsWith(rec))
                        {
                            if ((SelectionMode == SelectionMode.One && SelectedIndex == i) ||
                                (SelectionMode == SelectionMode.MultiSimple && SelectedIndices.Contains(i)) ||
                                (SelectionMode == SelectionMode.MultiExtended && SelectedIndices.Contains(i)))
                                OnDrawItem(new DrawItemEventArgs(g, Font, rec, i, DrawItemState.Selected, ForeColor, BackColor));
                            else
                                OnDrawItem(new DrawItemEventArgs(g, Font, rec, i, DrawItemState.Default, ForeColor, BackColor));

                            reg.Complement(rec);
                        }
                    }
                }

            }
        }
    }
}
