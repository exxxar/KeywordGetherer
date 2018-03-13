using MarkVSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordGetherer.Markov
{
    public class MarkovGen
    {
        protected GeneratorFacade gen;

        public MarkovGen(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Попробуйте указать правильный путь к файлу!");
            if ((new FileInfo(path)).Length<=0)
                throw new NullReferenceException("Файл пуст!");

            this.gen = new GeneratorFacade(new MarkovGenerator(System.IO.File.ReadAllText(path)));
        }

        public string paragrpaph(int size=1)
        {
            return this.gen.GenerateParagraphs(size);
        }

        public string sentence(int size = 1)
        {
            return this.gen.GenerateSentence(size);
        }

        public string title(int size = 1)
        {
            return this.gen.GenerateTitle(size);
        }

        public string words(int size = 1)
        {
            return this.gen.GenerateWords(size);
        }
    }
}
