using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace gcgcg
{
  internal class Circulo : Objeto
  {
    private double raio;
    private List<Ponto4D> pontosCirculo;

    public Circulo(Objeto _paiRef, ref char _rotulo, double _raio) : this(_paiRef, ref _rotulo, _raio, new Ponto4D())
    {

    }

    public Circulo(Objeto _paiRef, ref char _rotulo, double _raio, Ponto4D ptoDeslocamento) : base(_paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.LineLoop;
      PrimitivaTamanho = 2;
      this.raio = _raio;
      pontosCirculo = new List<Ponto4D>();

      for (int i = 0; i <= 360; i+=5)
      {
        Ponto4D ponto = Matematica.GerarPtosCirculo(i, _raio);
        pontosCirculo.Add(ponto);
        base.PontosAdicionar(ponto);
      }
      atualizarCalculoPontos(ptoDeslocamento);
    }

    public void CalcularPontosCirculo(Ponto4D ptoDeslocamento)
    {
        //sempre vai limpar a lista de pontos
        pontosCirculo.Clear();
        
        int numPontos = 72;
        
        double angulo = 360 / numPontos;
        double add = angulo;

        for (int i = 0; i < numPontos; i++)
        {
            Ponto4D ponto = new(Matematica.GerarPtosCirculo(angulo, raio).X + ptoDeslocamento.X, 
            Matematica.GerarPtosCirculo(angulo, raio).Y + ptoDeslocamento.Y);
            pontosCirculo.Add(ponto);
            angulo += add;
        }
    }

    //vai gerar os pontos novamente com o deslocamento e gerar o circulo
    public void atualizarCalculoPontos(Ponto4D ptoDeslocamento) 
    {
        CalcularPontosCirculo(ptoDeslocamento);
        pontosLista.Clear();        
        foreach (Ponto4D ponto in pontosCirculo) {
            PontosAdicionar(ponto);
        }
    }

    public void Atualizar(Ponto4D ptoDeslocamento)
    {
      double x = ptoDeslocamento.X;
      double y = ptoDeslocamento.Y;

      int i = 0;

        foreach (Ponto4D pto in pontosCirculo)
        {
            pto.X += x;
            pto.Y += y;

            PontosAlterar(pto, i);    
            i ++;    
        }
      base.ObjetoAtualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      System.Console.WriteLine("__________________________________ \n");
      string retorno;
      retorno = "__ Objeto Círculo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return retorno;
    }
#endif

  }
}
