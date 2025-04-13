using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic; // Necessário para List<>

namespace gcgcg
{
    internal class Ponto : Objeto
    {
        public Ponto(Objeto _paiRef, ref char _rotulo) : this(_paiRef, ref _rotulo, new Ponto4D())
        {
        }

        public Ponto(Objeto _paiRef, ref char _rotulo, Ponto4D pto) : base(_paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.Points;
            PrimitivaTamanho = 10;
            if (base.pontosLista == null) base.pontosLista = new List<Ponto4D>();
            base.pontosLista.Clear();
            base.pontosLista.Add(pto);
        }

        public void AtualizarGeometria()
        {
             if (base.pontosLista != null && base.pontosLista.Count > 0)
             {
                 base.ObjetoAtualizar();
             }
        }

        public void AtualizarPosicao(Ponto4D novaPosicao)
        {
            if (base.PontosListaTamanho > 0)
            {
                base.PontosAlterar(novaPosicao, 0);
            }
        }

#if CG_Debug
        public override string ToString()
        {
            System.Console.WriteLine("__________________________________ \n");
            string retorno;
            retorno = "__ Objeto Ponto _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return retorno;
        }
#endif
    }
}