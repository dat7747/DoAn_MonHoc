using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeThongTiemChungVaccine_NATHADA.Model
{
    class Model_ChitietCombo
    {
        private string maCombo;
        private string maLoaivaccine;
        private string maVaccine;
        private int soluong;

        public string MaCombo { get => maCombo; set => maCombo = value; }
        public string MaLoaivaccine { get => maLoaivaccine; set => maLoaivaccine = value; }
        public string MaVaccine { get => maVaccine; set => maVaccine = value; }
        public int Soluong { get => soluong; set => soluong = value; }
        public Model_ChitietCombo() { }
        public Model_ChitietCombo(string maCombo, string maLoaivaccine, string maVaccine, int soluong)
        {
            this.maCombo = maCombo;
            this.maLoaivaccine = maLoaivaccine;
            this.maVaccine = maVaccine;
            this.soluong = soluong;
        }
    }
}
