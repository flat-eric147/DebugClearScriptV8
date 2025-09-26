using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace TestClearScript
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private V8ScriptEngine? engine;
        private ScriptController scriptController;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = scriptController = new ScriptController();

            SetupScriptEngine();
            StartScriptEngine();
        }

        private void SetupScriptEngine()
        {
            engine = new V8ScriptEngine(
                         V8ScriptEngineFlags.EnableDebugging |
                         V8ScriptEngineFlags.EnableRemoteDebugging |
                         V8ScriptEngineFlags.EnableDynamicModuleImports |
                         V8ScriptEngineFlags.EnableTaskPromiseConversion |
                         V8ScriptEngineFlags.AddPerformanceObject |
                         V8ScriptEngineFlags.SetTimerResolution,
                         9224 /* the debug port */
                    );

            engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableAllLoading | DocumentAccessFlags.AllowCategoryMismatch;

            // this way in the script we can invoke testCL.WriteLog(...)
            engine.AddHostObject("testCL", HostItemFlags.GlobalMembers, scriptController);

            engine.Script.Global = engine.Script;
        }

        private void StopScriptEngine()
        {
            if (engine != null)
            {
                engine.Interrupt();
                engine.CollectGarbage(true);
                engine.Dispose();
                engine = null;
            }
        }

        private void StartScriptEngine()
        {
            // clear cache
            DocumentLoader.Default.DiscardCachedDocuments();

            // clear the log
            scriptController.ClearLog();

            // stop engine in case it's running
            if (engine != null)
                StopScriptEngine();

            SetupScriptEngine();

            // dump the script on file, and execute
            System.IO.File.WriteAllText("TestClearScript.js", scriptCode.Text);
            engine?.ExecuteDocument("TestClearScript.js", ModuleCategory.CommonJS);
        }

        private void OnStart(object sender, RoutedEventArgs e)
        {
            StartScriptEngine();
        }

        /* Stop ClearScript V8 engine */
        private void OnStop(object sender, RoutedEventArgs e)
        {
            StopScriptEngine();
        }

        /* invoke the method DoSomething with three arguments */
        private void OnDoSomething(object sender, RoutedEventArgs e)
        {
            try
            {
                engine?.Invoke("DoSomething", 1, 2, 3);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in DoSomething " + ex.Message);
            }
        }
    }
}