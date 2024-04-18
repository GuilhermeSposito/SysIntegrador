using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ClsEstiloComponentes
{
    public static void SetRoundedRegion(Control control, int radius) //Método para arredondar os cantos dos UserCntrol
    {
        GraphicsPath path = new GraphicsPath();
        int width = control.Width;
        int height = control.Height;
        path.AddArc(0, 0, radius, radius, 180, 90);
        path.AddArc(width - radius, 0, radius, radius, 270, 90);
        path.AddArc(width - radius, height - radius, radius, radius, 0, 90);
        path.AddArc(0, height - radius, radius, radius, 90, 90);
        path.CloseFigure();

        control.Region = new Region(path);
    }

    public static void CustomizePanelBorder(Panel panel) //comando que deixa o panel apenas com uma borda em baixo
    {
        // Definir o estilo da borda para None para que não haja borda padrão
        panel.BorderStyle = BorderStyle.None;

        // Manipular o evento Paint do painel para desenhar uma borda personalizada
        panel.Paint += (sender, e) =>
        {
            int borderWidth = 2; // Largura da borda
            Color borderColor = Color.Black; // Cor da borda

            // Calcular as coordenadas da borda inferior
            int y = panel.Height - borderWidth;
            int x1 = 0;
            int x2 = panel.Width;

            // Desenhar a borda inferior
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawLine(pen, x1, y, x2, y);
            }
        };
    }
}
