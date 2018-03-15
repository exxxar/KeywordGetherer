using MarkVSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KeywordGetherer.Markov
{
    public class MarkovGen
    {
        protected GeneratorFacade gen;
        protected string working_dir = "markov_rez";


        public MarkovGen(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Попробуйте указать правильный путь к файлу!");
            if ((new FileInfo(path)).Length<=0)
                throw new NullReferenceException("Файл пуст!");

            this.working_dir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + this.working_dir;
            this.gen = new GeneratorFacade(new MarkovGenerator(System.IO.File.ReadAllText(path)));

            if (!Directory.Exists(working_dir))
                Directory.CreateDirectory(working_dir);
        }

        public string paragrpaph(int size=1)
        {
            string str = this.gen.GenerateParagraphs(size);          
            File.WriteAllText(this.working_dir + "/" + DateTime.Now.Ticks + ".txt", str);
            return str;
        }

        public string sentence(int size = 1)
        {
            string str = this.gen.GenerateSentence(size);
            File.WriteAllText(this.working_dir + "/" + DateTime.Now.Ticks + ".txt", str);
            return str;
        }

        public string title(int size = 1)
        {
            string str = this.gen.GenerateTitle(size);
            File.WriteAllText(this.working_dir + "/" + DateTime.Now.Ticks + ".txt", str);
            return str;
        }

        public string words(int size = 1)
        {
            string str = this.gen.GenerateWords(size);
            File.WriteAllText(this.working_dir + "/" + DateTime.Now.Ticks + ".txt", str);
            return str;
        }
    }
}
