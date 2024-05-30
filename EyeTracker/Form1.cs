using System.Diagnostics;
using System.Management;

namespace EyeTracker
{
    public partial class Form1 : Form
    {
        List<string> cameraNames = new List<string>();
        string deviceProperty = "Caption";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        #region Methods
        public List<string> GetCameras()
        {
            Debug.WriteLine("TRYING TO LIST DEVICES");
            List<string> portnames = new List<string>();

            using (var searcher = new ManagementObjectSearcher(
                "SELECT * " +
                "FROM Win32_PnPEntity" +
                "WHERE Caption like '%(COM%'"))
            {
                foreach (var device in searcher.Get())
                {
                    if (device[deviceProperty] != null) continue;
                    #pragma warning disable CS8604 // Possible null reference argument.
                    portnames.Add(device[deviceProperty].ToString());
                    #pragma warning restore CS8604 // Possible null reference argument.
                }
            }
            return portnames;
        }
        #endregion

        #region Event Handlers
        private void btnGetCameras_Click(object sender, EventArgs e)
        {
            cameraNames = GetCameras();

            foreach (string name in cameraNames)
            {
                CamerasList.AppendText(name + "\r\n");
            }

        }
        private void btnGetExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

    }
}