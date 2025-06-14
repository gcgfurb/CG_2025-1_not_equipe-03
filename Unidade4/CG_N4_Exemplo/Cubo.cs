//https://github.com/mono/opentk/blob/main/Source/Examples/Shapes/Old/Cube.cs

#define CG_Debug
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;

namespace gcgcg
{
    internal class Cubo : Objeto
    {
        private Shader _shaderTextura;
        private Texture _textura;

        private Ponto4D[] _vertices;
        private Face[] _faces;

        public Cubo(Objeto _paiRef, ref char _rotulo, bool cuboMaior) : base(_paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.TriangleFan;
            PrimitivaTamanho = 10;

            if (cuboMaior)
                MontarCuboMaior(ref _rotulo);
            else
                MontarCuboMenor(ref _rotulo);

            Atualizar();
        }

        private void MontarCuboMaior(ref char rotulo)
        {
            _vertices = new[]
            {
                new Ponto4D(-1.0f, -1.0f, -1.0f), // Ponto 0
                new Ponto4D(-1.0f, -1.0f,  1.0f), // Ponto 1
                new Ponto4D(-1.0f,  1.0f,  1.0f), // Ponto 2
                new Ponto4D( 1.0f,  1.0f,  1.0f), // Ponto 3
                new Ponto4D( 1.0f,  1.0f, -1.0f), // Ponto 4
                new Ponto4D( 1.0f, -1.0f, -1.0f), // Ponto 5
                new Ponto4D(-1.0f,  1.0f, -1.0f), // Ponto 6
                new Ponto4D( 1.0f, -1.0f,  1.0f), // Ponto 7
            };

            var faceFrente = new Face(
                _paiRef: this,
                ref rotulo,
                vertices: new[] { _vertices[1], _vertices[7], _vertices[3], _vertices[3], _vertices[2], _vertices[1] },
                indicesTexturas: new[] { 3, 1, 0, 0, 2, 3 },
                indicesNormais: new[] { 2, 2, 2, 2, 2, 2 });

            var faceCima = new Face(
                _paiRef: this,
                ref rotulo,
                vertices: new[] { _vertices[6], _vertices[4], _vertices[3], _vertices[3], _vertices[2], _vertices[6] },
                indicesTexturas: new[] { 2, 0, 1, 1, 3, 2 },
                indicesNormais: new[] { 5, 5, 5, 5, 5, 5 });

            var faceFundo = new Face(
                _paiRef: this,
                ref rotulo,
                vertices: new[] { _vertices[0], _vertices[5], _vertices[4], _vertices[4], _vertices[6], _vertices[0] },
                indicesTexturas: new[] { 3, 1, 0, 0, 2, 3 },
                indicesNormais: new[] { 0, 0, 0, 0, 0, 0 });

            var faceBaixo = new Face(
                _paiRef: this,
                ref rotulo,
                vertices: new[] { _vertices[0], _vertices[5], _vertices[7], _vertices[7], _vertices[1], _vertices[0] },
                indicesTexturas: new[] { 2, 0, 1, 1, 3, 2 },
                indicesNormais: new[] { 4, 4, 4, 4, 4, 4 });

            var faceEsquerda = new Face(
                _paiRef: this,
                ref rotulo,
                vertices: new[] { _vertices[2], _vertices[6], _vertices[0], _vertices[0], _vertices[1], _vertices[2] },
                indicesTexturas: new[] { 2, 0, 1, 1, 3, 2 },
                indicesNormais: new[] { 3, 3, 3, 3, 3, 3 });

            var faceDireita = new Face(
                _paiRef: this,
                ref rotulo,
                vertices: new[] { _vertices[3], _vertices[4], _vertices[5], _vertices[5], _vertices[7], _vertices[3] },
                indicesTexturas: new[] { 2, 0, 1, 1, 3, 2 },
                indicesNormais: new[] { 1, 1, 1, 1, 1, 1 });

            _shaderTextura = new Shader("Shaders/shaderTextura.vert", "Shaders/shaderTextura.frag");
            _textura = Texture.LoadFromFile("resources/dinossauros.png");

            faceFrente.shaderCor = _shaderTextura;
            faceCima.shaderCor = _shaderTextura;
            faceFundo.shaderCor = _shaderTextura;
            faceBaixo.shaderCor = _shaderTextura;
            faceEsquerda.shaderCor = _shaderTextura;
            faceDireita.shaderCor = _shaderTextura;

            _faces = new[]
            {
                faceFrente,
                faceCima,
                faceFundo,
                faceBaixo,
                faceEsquerda,
                faceDireita,
            };
        }

        private void MontarCuboMenor(ref char rotulo)
        {
            var centro = new Ponto4D(0.0, 0.0);
            var tamanhoLado = 1.0d;

            var metadeLado = tamanhoLado / 2;
            var maxX = centro.X + metadeLado;
            var minX = centro.X - metadeLado;
            var maxY = centro.Y + metadeLado;
            var minY = centro.Y - metadeLado;
            var maxZ = centro.Z + metadeLado;
            var minZ = centro.Z - metadeLado;

            _vertices = new[]
            {
                new Ponto4D(minX, maxY, minZ), // Ponto 0
                new Ponto4D(maxX, maxY, minZ), // Ponto 1
                new Ponto4D(maxX, maxY, maxZ), // Ponto 2
                new Ponto4D(minX, maxY, maxZ), // Ponto 3
                new Ponto4D(minX, minY, minZ), // Ponto 4
                new Ponto4D(maxX, minY, minZ), // Ponto 5
                new Ponto4D(maxX, minY, maxZ), // Ponto 6
                new Ponto4D(minX, minY, maxZ), // Ponto 7
            };

            var faceFrente = new Face(
                _paiRef: this,
                ref rotulo,
                vertices: new[] { _vertices[3], _vertices[2], _vertices[6], _vertices[6], _vertices[7], _vertices[3] });

            var faceCima = new Face(
                _paiRef: this,
                ref rotulo,
                vertices: new[] { _vertices[0], _vertices[1], _vertices[2], _vertices[2], _vertices[3], _vertices[0] });

            var faceFundo = new Face(
                _paiRef: this,
                ref rotulo,
                vertices: new[] { _vertices[0], _vertices[1], _vertices[5], _vertices[5], _vertices[4], _vertices[0] });

            var faceBaixo = new Face(
                _paiRef: this,
                ref rotulo,
                vertices: new[] { _vertices[4], _vertices[5], _vertices[6], _vertices[6], _vertices[7], _vertices[4] });

            var faceEsquerda = new Face(
                _paiRef: this,
                ref rotulo,
                vertices: new[] { _vertices[3], _vertices[0], _vertices[4], _vertices[4], _vertices[7], _vertices[3] });

            var faceDireita = new Face(
                _paiRef: this,
                ref rotulo,
                vertices: new[] { _vertices[2], _vertices[1], _vertices[5], _vertices[5], _vertices[6], _vertices[2] });

            _faces = new[]
            {
                faceFrente,
                faceCima,
                faceFundo,
                faceBaixo,
                faceEsquerda,
                faceDireita,
            };

            base.MatrizTranslacaoXYZ(3.0, 0.0, 0.0);
        }

        private void Atualizar()
        {
            base.ObjetoAtualizar();
        }

#if CG_Debug
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Cubo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return (retorno);
        }
#endif
    }
}