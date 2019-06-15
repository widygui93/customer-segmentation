using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
namespace PosProject
{
    class Proses
    {
        MySqlConnection con = new MySqlConnection("server=localhost;User Id=root;Persist Security Info=True;database=POS");
        MySqlCommand command;
        string CodeWords = "";
        int[] KumpulanCodeWords ;
        int[] KumpulanCodeWordsYangIsinyaUnik;
        double[] cw = new double[3];
        double[] cwBaru = new double[3];

        List<int> Regular = new List<int>();
        List<int> Potential = new List<int>();
        List<int> VIP = new List<int>();

        List<int> RegularTemp = new List<int>();
        List<int> PotentialTemp = new List<int>();
        List<int> VIPTemp = new List<int>();

        List<int> RegularBaru = new List<int>();
        List<int> PotentialBaru = new List<int>();
        List<int> VIPBaru = new List<int>();

        bool clustersatu, clusterdua, clustertiga;

        public void openConnection()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
        }

        public void closeConnection()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }

        public string JumlahCodeWord()
        {

            openConnection();
            string myquery = "select count(*) from pos.cust_trns";
            command = new MySqlCommand(myquery, con);
            CodeWords = Convert.ToString(command.ExecuteScalar());
            closeConnection();

            return CodeWords;
        }


        public double[] CodeWord2()
        {
            List<int> vektorCluster1 = new List<int>();
            List<int> vektorCluster3 = new List<int>();

            double totalJumlahVektor = 0;
            double totalJumlahVektorCluster1 = 0;
            double totalJumlahVektorCluster3 = 0;

            KumpulanCodeWord();

            
            for (int i = 0; i < KumpulanCodeWordsYangIsinyaUnik.Length; i++)
            {
                totalJumlahVektor = KumpulanCodeWordsYangIsinyaUnik[i] + totalJumlahVektor;
            }

            cw[1] = totalJumlahVektor / KumpulanCodeWordsYangIsinyaUnik.Length;

            for (int i = 0; i < KumpulanCodeWordsYangIsinyaUnik.Length; i++)
            {
                if (KumpulanCodeWordsYangIsinyaUnik[i] < cw[1])
                {
                    vektorCluster1.Add(KumpulanCodeWordsYangIsinyaUnik[i]);
                }
                else if (KumpulanCodeWordsYangIsinyaUnik[i] > cw[1])
                {
                    vektorCluster3.Add(KumpulanCodeWordsYangIsinyaUnik[i]);
                }

            }

            for (int i = 0; i < vektorCluster1.Count; i++)
            {
                totalJumlahVektorCluster1 = vektorCluster1[i] + totalJumlahVektorCluster1;
            }

            cw[0] = totalJumlahVektorCluster1 / vektorCluster1.Count;

            for (int i = 0; i < vektorCluster3.Count; i++)
            {
                totalJumlahVektorCluster3 = vektorCluster3[i] + totalJumlahVektorCluster3;
            }

            cw[2] = totalJumlahVektorCluster3 / vektorCluster3.Count;

            return cw;
        }

        public int[] KumpulanCodeWord()
        {
            int idx = 0;
            JumlahCodeWord();
            KumpulanCodeWords = new int[int.Parse(CodeWords)];
            openConnection();
            MySqlDataAdapter sda = new MySqlDataAdapter("SELECT Total_Belanja FROM pos.cust_trns", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {

                KumpulanCodeWords[idx] = int.Parse(row["Total_Belanja"].ToString());
                idx = idx + 1;
            }

            //do not insert same totalbelanja into array 
            //so all vektor become unique
            KumpulanCodeWordsYangIsinyaUnik = KumpulanCodeWords.Distinct().ToArray();

            
            closeConnection();


            return KumpulanCodeWordsYangIsinyaUnik;
        }


        public List<List<int>> VQ()
        {
            CodeWord2();

            // (yang 1) MessageBox.Show("codeword1 yang pertama kali adalah" + Math.Round(cw[0], 2));
            // (yang 2) MessageBox.Show("codeword2 yang pertama kali adalah" + Math.Round(cw[1], 2));
            // (yang 3) MessageBox.Show("codeword3 yang pertama kali adalah" + Math.Round(cw[2], 2));

            double[] euclidean = new double[3];
            
            KumpulanCodeWord();
            for (int a = 0; a < KumpulanCodeWordsYangIsinyaUnik.Length; a++)
            {
                for (int b = 0; b < 3;b++ )
                {
                    euclidean[b] = Math.Sqrt(Math.Pow(KumpulanCodeWordsYangIsinyaUnik[a] - cw[b], 2));
                    // (yang 4) MessageBox.Show("Vektor " + KumpulanCodeWordsYangIsinyaUnik[a].ToString() + " terhadap " + Math.Round(cw[b], 2) + " adalah " + Math.Round(euclidean[b],2));
                    
                    
                }
                // (yang 5) MessageBox.Show("Jarak terdekat vektor "+KumpulanCodeWordsYangIsinyaUnik[a].ToString()+" adalah "+ Math.Round(euclidean.Min(),2));
                
                int minIndex = Array.IndexOf(euclidean, euclidean.Min());

                if(minIndex==0)
                {
                    Regular.Add(KumpulanCodeWordsYangIsinyaUnik[a]);
                }
                else if (minIndex == 1)
                {
                    Potential.Add(KumpulanCodeWordsYangIsinyaUnik[a]);
                }
                else
                {
                    VIP.Add(KumpulanCodeWordsYangIsinyaUnik[a]);
                }
            }

            foreach (int cl1 in Regular)
            {
                //MessageBox.Show("Cluster Regular adalah " + cl1.ToString());
                RegularTemp.Add(cl1);
            }

            foreach (int cl2 in Potential)
            {
                //MessageBox.Show("Cluster Regular adalah " + cl2.ToString());
                PotentialTemp.Add(cl2);
            }

            foreach (int cl3 in VIP)
            {
                //MessageBox.Show("Cluster Regular adalah " + cl3.ToString());
                VIPTemp.Add(cl3);
            }

            do{
                
                cwBaru[0] = Convert.ToDouble(RegularTemp.Sum()) / RegularTemp.Count;
                // (yang 6) MessageBox.Show("codeword 1 yg baru adalah "+Math.Round(cwBaru[0], 2));

                cwBaru[1] = Convert.ToDouble(PotentialTemp.Sum()) / PotentialTemp.Count;
                // (yang 7) MessageBox.Show("codeword 2 yg baru adalah "+Math.Round(cwBaru[1], 2));

                cwBaru[2] = Convert.ToDouble(VIPTemp.Sum()) / VIPTemp.Count;
                // (yang 8) MessageBox.Show("codeword 3 yg baru adalah "+Math.Round(cwBaru[2], 2));

                RegularBaru.Clear();
                PotentialBaru.Clear();
                VIPBaru.Clear();

                //do it again,calculate euclidean against all vektor
                for (int a = 0; a < KumpulanCodeWordsYangIsinyaUnik.Length; a++)
                {
                    for (int b = 0; b < 3;b++ )
                    {
                        euclidean[b] = Math.Sqrt(Math.Pow(KumpulanCodeWordsYangIsinyaUnik[a] - cwBaru[b], 2));
                        // (yang 9) MessageBox.Show("Vektor " + KumpulanCodeWordsYangIsinyaUnik[a].ToString() + " terhadap " + Math.Round(cwBaru[b],2) + " adalah " + Math.Round(euclidean[b],2));
                    
                    
                    }
                    // (yang 10) MessageBox.Show("Jarak terdekat vektor "+KumpulanCodeWordsYangIsinyaUnik[a].ToString()+" adalah "+Math.Round(euclidean.Min(),2));
                
                    int minIndex = Array.IndexOf(euclidean, euclidean.Min());

                    if(minIndex==0)
                    {
                        RegularBaru.Add(KumpulanCodeWordsYangIsinyaUnik[a]);
                    }
                    else if (minIndex == 1)
                    {
                        PotentialBaru.Add(KumpulanCodeWordsYangIsinyaUnik[a]);
                    }
                    else
                    {
                        VIPBaru.Add(KumpulanCodeWordsYangIsinyaUnik[a]);
                    }
                }

                clustersatu = new HashSet<int>(RegularTemp).SetEquals(RegularBaru);
                clusterdua = new HashSet<int>(PotentialTemp).SetEquals(PotentialBaru);
                clustertiga = new HashSet<int>(VIPTemp).SetEquals(VIPBaru);
                

                foreach (int cls1 in RegularTemp)
                {
                    // (yang 11) MessageBox.Show("Cluster RegularTemp adalah " + cls1.ToString());
                   
                }
                foreach (int cls1 in PotentialTemp)
                {
                    // (yang 12) MessageBox.Show("Cluster PotentialTemp adalah " + cls1.ToString());

                }
                foreach (int cls1 in VIPTemp)
                {
                    // (yang 13) MessageBox.Show("Cluster VIPTemp adalah " + cls1.ToString());

                }

                foreach (int cls1 in RegularBaru)
                {
                    // (yang 14) MessageBox.Show("Cluster RegularBaru adalah " + cls1.ToString());

                }
                foreach (int cls1 in PotentialBaru)
                {
                    // (yang 15) MessageBox.Show("Cluster PotentialBaru adalah " + cls1.ToString());

                }
                foreach (int cls1 in VIPBaru)
                {
                    // (yang 16) MessageBox.Show("Cluster VIPBaru adalah " + cls1.ToString());

                }

                // (yang 17) MessageBox.Show("cluster 1 yang baru dan lama adalah" +clustersatu);
                // (yang 18) MessageBox.Show("cluster 2 yang baru dan lama adalah" + clusterdua);
                // (yang 19) MessageBox.Show("cluster 3 yang baru dan lama adalah" + clustertiga);

                RegularTemp.Clear();
                PotentialTemp.Clear();
                VIPTemp.Clear();

                foreach (int cls1 in RegularBaru)
                {
                    //MessageBox.Show("Cluster Regular adalah " + cl1.ToString());
                    RegularTemp.Add(cls1);
                }

                foreach (int cls2 in PotentialBaru)
                {
                    //MessageBox.Show("Cluster Regular adalah " + cl2.ToString());
                    PotentialTemp.Add(cls2);
                }

                foreach (int cls3 in VIPBaru)
                {
                    //MessageBox.Show("Cluster Regular adalah " + cl3.ToString());
                    VIPTemp.Add(cls3);
                }

            } while (clustersatu == false || clusterdua == false || clustertiga == false);
            
            List<List<int>> HasilDaftar = new List<List<int>>();
            HasilDaftar.Add(RegularBaru);
            HasilDaftar.Add(PotentialBaru);
            HasilDaftar.Add(VIPBaru);


            return HasilDaftar;
        }
    }
}
