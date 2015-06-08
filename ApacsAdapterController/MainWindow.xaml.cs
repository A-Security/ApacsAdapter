using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ApacsAdapter;
using System.IO;

namespace ApacsAdapterController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AdpLog log;
        private ApcServer Apacs;
        private AdpCfgXml cfg;
        private ApcData apcData;
        public MainWindow()
        {
            AdpLog.OnAddLog += new EventHandler(AdpLog_OnAddLog);
            log = new AdpLog();
            cfg = new AdpCfgXml();
            InitializeComponent();
            Apacs = new ApcServer(cfg.apcLogin, cfg.apcPasswd);
            apcData = new ApcData();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            apcSubscribe();
        }
        private void apcSubscribe()
        {
            try
            {
                Apacs.ApacsDisconnect += new ApcServer.ApacsDisconnectHandler(onDisconnect);
                Apacs.ApacsEvent += new ApcServer.ApacsEventHandler(onEvent);
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
        }
        private void apcUnsubscribe()
        {
            try
            {
                Apacs.ApacsEvent -= new ApcServer.ApacsEventHandler(onEvent);
                Apacs.ApacsDisconnect -= new ApcServer.ApacsDisconnectHandler(onDisconnect);
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
        }
        private void onDisconnect()
        {
            apcUnsubscribe();
            Apacs.Dispose();
            Apacs = new ApcServer(cfg.apcLogin, cfg.apcPasswd);
            apcSubscribe();
        }

        private void onEvent(ApcPropObj evtSets)
        {
            string eventType = evtSets.getEventTypeProperty();
            if (!eventType.StartsWith(ApcObjType.TApcCardHolderAccess) || eventType.Contains("Will"))
            {
                return;
            }
            ApcObj holder = evtSets.getSysAddrHolderProperty();
            AdpAPCEvtObj evnt = apcData.mapAdpAPCEvtObj(evtSets);
            BitmapImage biImg = new BitmapImage();
            biImg.BeginInit();
            biImg.StreamSource = new MemoryStream(holder.getMainPhoto());
            biImg.EndInit();
            mainPhoto.Source = biImg as ImageSource;
        }

        private static void AdpLog_OnAddLog(object sender, EventArgs arg)
        {
            MessageBox.Show(((AdpLog)sender).Log);
        }
    }
}
