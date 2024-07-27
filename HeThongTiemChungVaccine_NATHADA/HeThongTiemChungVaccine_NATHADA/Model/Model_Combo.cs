using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeThongTiemChungVaccine_NATHADA.Model
{
    class Model_Combo
    {
        private string maCombo;
        private string tenCombo;
        private float gia;

        public string MaCombo { get => maCombo; set => maCombo = value; }
        public string TenCombo { get => tenCombo; set => tenCombo = value; }
        public float Gia { get => gia; set => gia = value; }
    
        public Model_Combo() { }
        public Model_Combo(string maCombo, string tenCombo, float gia)
        {
            this.maCombo = maCombo;
            this.tenCombo = tenCombo;
            this.gia = gia;
        }
    }
}
