using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    internal class SrPalito : Objeto
    {
        double raio = 0.5;
        double angulo = 45;
        double posicaoX = 0;
        double posicaoY = 0;

        public SrPalito(Objeto _paiRef, char _rotulo) : base(_paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.Lines;
            PrimitivaTamanho = 2;
            Atualizar();
        }

        public void Atualizar()
        {
            // toda vez é necessário limpar para renderizar o quadro atualizado
            pontosLista.Clear();
            // gera novos pontos para o frame
            Ponto4D pontoInicial = Matematica.GerarPtosCirculo(angulo, raio);
            // atualiza as coordenadas do ponto inicial e adiciona no frame
            pontoInicial.X += posicaoX;
            pontoInicial.Y += posicaoY;
            PontosAdicionar(new Ponto4D(posicaoX, posicaoY));
            PontosAdicionar(pontoInicial);
            ObjetoAtualizar();
        }

        public void AtualizarPe(double peInc)
        {
            posicaoX += peInc;
            Atualizar();
        }

        public void AtualizarRaio(double raioInc)
        {
            raio += raioInc;
            Atualizar();
        }

        public void AtualizarAngulo(double anguloInc)
        {
            angulo += anguloInc;
            Atualizar();
        }

#if CG_Debug
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto SrPalito _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return (retorno);
        }
#endif
    }
}
