using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic; // Necessário para List<>

namespace gcgcg
{
    internal class SegReta : Objeto
    {
        public SegReta(Objeto _paiRef, ref char _rotulo) : this(_paiRef, ref _rotulo, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5))
        {
        }

        public SegReta(Objeto _paiRef, ref char _rotulo, Ponto4D ptoIni, Ponto4D ptoFim) : base(_paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.Lines;
            PrimitivaTamanho = 1;
            if (base.pontosLista == null) base.pontosLista = new List<Ponto4D>();
            base.pontosLista.Clear();
            base.pontosLista.Add(ptoIni);
            base.pontosLista.Add(ptoFim);
        }

        public void AtualizarGeometria()
        {
             if (base.pontosLista != null && base.pontosLista.Count > 1)
             {
                 base.ObjetoAtualizar();
             }
        }

        public void AtualizarPontos(Ponto4D ptoInicial, Ponto4D ptoFinal)
        {
            if (base.PontosListaTamanho > 1)
            {
                base.PontosAlterar(ptoInicial, 0);
                base.PontosAlterar(ptoFinal, 1);
            }
        }

#if CG_Debug
        public override string ToString()
        {
            System.Console.WriteLine("__________________________________ \n");
            string retorno;
            retorno = "__ Objeto SegReta _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return retorno;
        }
#endif
    }
}