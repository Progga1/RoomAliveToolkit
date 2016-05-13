using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoomAliveTestApp {
	public partial class AppForm : Form {
		public AppForm() {
			InitializeComponent();
		}

		private void AppForm_Load(object sender,EventArgs e) {

		}

		private void OnFormClosed(object sender,FormClosingEventArgs e) {
			Environment.Exit(0);
		}

	}
}
