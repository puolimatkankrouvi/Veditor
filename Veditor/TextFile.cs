using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsForms
{
    /*
       Tietorakenne, jossa textPanel <-> filename,
       joka on yhdistetty tabiin
    */
    class TextFile
    {
        private String filename;
        private byte[] contents;


        public TextFile(){
            filename = "";
            contents = null;
        }

        public TextFile(String fname, byte[] contents) {
            this.contents = contents;
            filename = fname;
        }


        public String getFilename()
        {
            return filename;
        }
        public void setFilename(String fname)
        {
            filename = fname;
        }

        public byte[] readContent()
        {
            return contents;
        }

        public String readContentString()
        {
            return Encoding.UTF8.GetString(contents);
        }

        public void setContents(byte[] c)
        {
            contents = c;
        }
    }
}
