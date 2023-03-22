using Linearstar.Windows.RawInput;

namespace RawInput.Sharp.DigitizerExample;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        RawInputDevice.RegisterDevice(HidUsageAndPage.Pen, RawInputDeviceFlags.None, this.Handle);
        RawInputDevice.RegisterDevice(HidUsageAndPage.TouchScreen, RawInputDeviceFlags.None, this.Handle);
        RawInputDevice.RegisterDevice(HidUsageAndPage.TouchPad, RawInputDeviceFlags.None, this.Handle);
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_INPUT = 0x00FF;

        if (m.Msg == WM_INPUT)
        {
            var data = RawInputData.FromHandle(m.LParam);

            if (data is RawInputDigitizerData digitizerData)
            {
                var contacts = digitizerData.Contacts;

                this.textLabel.Text =
                    $"Touch anywhere in this window.\r\n\r\nContacts: {digitizerData.ContactsCount}\r\n"
                    + string.Join("\r\n", contacts.Select(x => x.ToString()));
            }
        }

        base.WndProc(ref m);
    }
}