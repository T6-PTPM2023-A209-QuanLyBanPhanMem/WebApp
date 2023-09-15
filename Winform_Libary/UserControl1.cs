﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Winform_Libary
{
    public partial class UserControl1: UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            this.txt_masv.TextChanged += txt_masv_TextChanged;
        }

        void txt_masv_TextChanged(object sender, EventArgs e)
        {
            docfile(txt_masv.Text);
        }

        void docfile(string masv)
        {
            string path = "E:\\dssv.txt";

            foreach(string line in File.ReadLines(path))
            {
                string[] parts = line.Split(',');
                if(String.Compare(masv, parts[0], true) == 0)
                {
                    lbl_masv.Text = parts[0];
                    lbl_tensv.Text = parts[1];
                    lbl_malop.Text = parts[2];
                    lbl_ngaysinh.Text = parts[3];
                    lbl_gioitinh.Text = parts[4];
                    lbl_cmnd.Text = parts[5];
                    return;
                }

            }
        }


    }
}
