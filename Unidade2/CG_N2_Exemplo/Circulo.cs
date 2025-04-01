using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    internal class Circulo : Objeto
    {
        public Circulo(Objeto _paiRef, ref char _rotulo) : this(_paiRef, ref _rotulo, new Ponto4D(0.5, 0.5))
        {

        }

        public Circulo(Objeto _paiRef, ref char _rotulo, Ponto4D ptoSupDir) : base(_paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.Points;
            PrimitivaTamanho = 70;

            base.PontosAdicionar(ptoSupDir);
            base.PontosAdicionar(new Ponto4D(ptoSupDir.X, ptoSupDir.Y));
            Atualizar();
        }

        private void Atualizar()
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
