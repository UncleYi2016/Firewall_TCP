using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewall.Models {
    class LogTable : ITable {
        private string _filePath = "./log.txt";
        public string FilePath {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public LogTable() {

        }

        public LogTable(string filePath) {
            try {
                FilePath = filePath;
            } catch (Exception ex) {
                
            }
        }

        public void write(string content) {
            StreamWriter sw = File.AppendText(FilePath);
            sw.WriteLine(content);
            sw.Close();
        }

        public ArrayList read() {
            throw new NotImplementedException();
        }

        public void modify() {
            throw new NotImplementedException();
        }

        public void delete() {
            throw new NotImplementedException();
        }
    }
}
