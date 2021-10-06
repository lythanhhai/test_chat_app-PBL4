﻿using PBL4_Chat.BLL;
using PBL4_Chat.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PBL4_Chat.View
{
    public partial class user_info : UserControl
    {
        public user_info()
        {
            InitializeComponent();
        }
        private string _userId;
        private string _name;
        private string _phone;

        public string userId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
            }
        }
        public string name
        {
            get
            {
                return lbName.Text;
            }
            set
            {
                lbName.Text = value;
            }
        }



        public string phone
        {
            get
            {
                return lbPhone.Text;
            }
            set
            {
                lbPhone.Text = value;
            }
        }


        public string getUserId()
        {
            return this.userId;
        }

        public event EventHandler DataAvailable;
        /// <summary>
        /// Called to signal to subscribers that new data is available
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDataAvailable(EventArgs e)
        {
            EventHandler eh = DataAvailable;
            if (eh != null)
            {
                eh(this, e);
            }
        }
        private void gunaGradientPanel1_Click(object sender, EventArgs e)
        {
            // chuyển dữ liệu
            ((mainForm)this.ParentForm).userId_receive = new mainForm.getUserIdReveive(getUserId);
            // load tên
            ((mainForm)this.ParentForm).lbName_Receiver.Text = this.name;
            ((mainForm)this.ParentForm).lbStatus.Text = "online";
            ((mainForm)this.ParentForm).pn_chat.Visible = true;
            BLL_UserRelation.instance.BLL_loadMessageForChat(((mainForm)this.ParentForm).userId(), this.userId);
        }
    }
}
