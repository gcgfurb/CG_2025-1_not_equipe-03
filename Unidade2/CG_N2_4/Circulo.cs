using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Circulo : Objeto
  {
    public Circulo(Objeto _paiRef, ref char _rotulo, double _raio) : this(_paiRef, ref _rotulo, _raio, new Ponto4D())
    {

    }

    public Circulo(Objeto _paiRef, ref char _rotulo, double _raio, Ponto4D ptoDeslocamento) : base(_paiRef, ref _rotulo)
    {
      PrimitivaTamanho = 5;

      for (int i = 0; i <= 360; i+=5)
      {
        Ponto4D ponto = Matematica.GerarPtosCirculo(i, _raio);
        base.PontosAdicionar(ponto);
      }
    }

    private void Atualizar(Ponto4D ptoDeslocamento)
    {
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
