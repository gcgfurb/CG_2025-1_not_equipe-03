using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Spline : Objeto
  {
    private List<Ponto4D> controlPoints = new List<Ponto4D>();
    private List<Ponto4D> splinePoints = new List<Ponto4D>();
    private int selectedControlPointIndex = 0;
    public int SelectedControlPointIndex => selectedControlPointIndex;
    private int numSplinePoints = 20;

    public Spline(Objeto _paiRef, ref char _rotulo, List<Ponto4D> _controlPoints) : base(_paiRef, ref _rotulo)
    {
        PrimitivaTipo = PrimitiveType.LineStrip;
        PrimitivaTamanho = 1;
        controlPoints = _controlPoints;

        CalcularPontosSpline();
        Atualizar();
    }        

    private void CalcularPontosSpline()
    {
        splinePoints.Clear();

        if (controlPoints.Count < 2)
        {
            return;
        }

        for (int i = 0; i < numSplinePoints; i++)
        {
            double t = (double)i / (numSplinePoints - 1);
            Ponto4D ponto = CalcularPontoBezier(t);
            splinePoints.Add(ponto);
        }
    }

    private Ponto4D CalcularPontoBezier(double t)
    {
        if (controlPoints.Count == 2)
        {
            double x = (1 - t) * controlPoints[0].X + t * controlPoints[1].X;
            double y = (1 - t) * controlPoints[0].Y + t * controlPoints[1].Y;
            return new Ponto4D(x, y);
        }
        else if (controlPoints.Count == 3)
        {
            double x = (1 - t) * (1 - t) * controlPoints[0].X + 2 * (1 - t) * t * controlPoints[1].X + t * t * controlPoints[2].X;
            double y = (1 - t) * (1 - t) * controlPoints[0].Y + 2 * (1 - t) * t * controlPoints[1].Y + t * t * controlPoints[2].Y;
            return new Ponto4D(x, y);
        }
        else if (controlPoints.Count == 4)
        {
            double x = (1 - t) * (1 - t) * (1 - t) * controlPoints[0].X + 3 * (1 - t) * (1 - t) * t * controlPoints[1].X + 3 * (1 - t) * t * t * controlPoints[2].X + t * t * t * controlPoints[3].X;
            double y = (1 - t) * (1 - t) * (1 - t) * controlPoints[0].Y + 3 * (1 - t) * (1 - t) * t * controlPoints[1].Y + 3 * (1 - t) * t * t * controlPoints[2].Y + t * t * t * controlPoints[3].Y;
            return new Ponto4D(x, y);
        }
        else
        {
            double x = (1 - t) * controlPoints[0].X + t * controlPoints[1].X;
            double y = (1 - t) * controlPoints[0].Y + t * controlPoints[1].Y;
            return new Ponto4D(x, y);
        }
    }

    public void MoverPontoControle(int index, double xInc, double yInc)
    {
        controlPoints[index].X += xInc;
        controlPoints[index].Y += yInc;
        CalcularPontosSpline();
        Atualizar();
    }

    public void TrocarPontoDeControleSelecionado()
    {
        selectedControlPointIndex = (selectedControlPointIndex + 1) % controlPoints.Count;
    }

    public void MudarNumeroPontosCurva(int increment)
    {
        numSplinePoints += increment;
        if (numSplinePoints < 2) numSplinePoints = 2; // Ensure at least 2 points
        CalcularPontosSpline();
        Atualizar();
    }

    private void Atualizar()
    {
        base.pontosLista.Clear();

        foreach (var ponto in splinePoints)
        {
            base.PontosAdicionar(ponto);
        }

        base.ObjetoAtualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      System.Console.WriteLine("__________________________________ \n");
      string retorno;
      retorno = "__ Objeto Spline _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return retorno;
    }
#endif

  }
}
