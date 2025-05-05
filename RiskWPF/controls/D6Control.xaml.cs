using System;
using System.Windows;
using System.Windows.Controls;

namespace RiskWPF.controls
{
    public partial class D6Control : UserControl
    {
        public D6Control()
        {
            InitializeComponent();
            Loaded += D6Control_Loaded;
        }

        public int MyNumber
        {
            get { return (int)GetValue(MyNumberProperty); }
            set { SetValue(MyNumberProperty, value); }
        }

        public static readonly DependencyProperty MyNumberProperty =
            DependencyProperty.Register("MyNumber", typeof(int), typeof(D6Control), new PropertyMetadata(1, OnMyNumberChanged));

        private async void D6Control_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await webView.EnsureCoreWebView2Async();

                string htmlContent = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            height: 100vh;
                            margin: 0;
                        }
                        .scene {
                            width: 100px;
                            height: 100px;
                            perspective: 500px;
                        }
                        .cube {
                            width: 100px;
                            height: 100px;
                            position: relative;
                            transform-style: preserve-3d;
                            transition: transform 2s;
                        }
                        .face {
                            position: absolute;
                            width: 100px;
                            height: 100px;
                            background-color: white;
                            border: 1px solid black;
                            display: flex;
                            justify-content: center;
                            align-items: center;
                        }
                        .dot {
                            position: absolute;
                            width: 15px;
                            height: 15px;
                            background-color: black;
                            border-radius: 50%;
                        }
                        .one .dot { top: 37px; left: 37px; }
                        .two .dot:nth-child(1) { top: 15px; left: 15px; }
                        .two .dot:nth-child(2) { top: 60px; left: 60px; }
                        .three .dot:nth-child(1) { top: 15px; left: 15px; }
                        .three .dot:nth-child(2) { top: 37px; left: 37px; }
                        .three .dot:nth-child(3) { top: 60px; left: 60px; }
                        .four .dot:nth-child(1) { top: 15px; left: 15px; }
                        .four .dot:nth-child(2) { top: 15px; right: 15px; }
                        .four .dot:nth-child(3) { top: 60px; left: 15px; }
                        .four .dot:nth-child(4) { top: 60px; right: 15px; }
                        .five .dot:nth-child(1) { top: 15px; left: 15px; }
                        .five .dot:nth-child(2) { top: 15px; right: 15px; }
                        .five .dot:nth-child(3) { top: 37px; left: 37px; }
                        .five .dot:nth-child(4) { top: 60px; left: 15px; }
                        .five .dot:nth-child(5) { top: 60px; right: 15px; }
                        .six .dot:nth-child(1) { top: 15px; left: 15px; }
                        .six .dot:nth-child(2) { top: 15px; right: 15px; }
                        .six .dot:nth-child(3) { top: 37px; left: 15px; }
                        .six .dot:nth-child(4) { top: 37px; right: 15px; }
                        .six .dot:nth-child(5) { top: 60px; left: 15px; }
                        .six .dot:nth-child(6) { top: 60px; right: 15px; }
        
                        .one   { transform: rotateY(0deg) translateZ(50px); }
                        .two   { transform: rotateY(90deg) translateZ(50px); }
                        .three { transform: rotateY(180deg) translateZ(50px); }
                        .four  { transform: rotateY(-90deg) translateZ(50px); }
                        .five  { transform: rotateX(-90deg) translateZ(50px); }
                        .six   { transform: rotateX(90deg) translateZ(50px); }
                    </style>
                </head>
                <body>
                    <div class=""scene"">
                        <div class=""cube"" id=""cube"">
                            <div class=""face one""><div class=""dot""></div></div>
                            <div class=""face two""><div class=""dot""></div><div class=""dot""></div></div>
                            <div class=""face three""><div class=""dot""></div><div class=""dot""></div><div class=""dot""></div></div>
                            <div class=""face four""><div class=""dot""></div><div class=""dot""></div><div class=""dot""></div><div class=""dot""></div></div>
                            <div class=""face five""><div class=""dot""></div><div class=""dot""></div><div class=""dot""></div><div class=""dot""></div><div class=""dot""></div></div>
                            <div class=""face six""><div class=""dot""></div><div class=""dot""></div><div class=""dot""></div><div class=""dot""></div><div class=""dot""></div><div class=""dot""></div></div>
                        </div>
                    </div>

                    <script>
                        function rollDice(number) {
                            let xEnd = 0;
                            let yEnd = 0;

                            switch (number) {
                                case 1: xEnd = 0; yEnd = 0; break;
                                case 2: xEnd = 0; yEnd = -90; break;
                                case 3: xEnd = 0; yEnd = 180; break;
                                case 4: xEnd = 0; yEnd = 90; break;
                                case 6: xEnd = -90; yEnd = 0; break;
                                case 5: xEnd = 90; yEnd = 0; break;
                            }

                            const cube = document.getElementById('cube');
                            cube.style.transform = `rotateX(${xEnd}deg) rotateY(${yEnd}deg)`;
                        }
                    </script>
                </body>
                </html>";

                webView.NavigateToString(htmlContent);
                await UpdateDiceValue(MyNumber);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error initializing WebView2: " + ex.Message);
            }
            
        }

        private static async void OnMyNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is D6Control control)
            {
                if (control.webView.CoreWebView2 != null)
                {
                    int newNumber = (int)e.NewValue;
                    await control.UpdateDiceValue(newNumber);
                }
            }
        }

        private async System.Threading.Tasks.Task UpdateDiceValue(int number)
        {
            if (webView.CoreWebView2 != null)
            {
                await webView.CoreWebView2.ExecuteScriptAsync($"rollDice({number});");
            }
        }
    }
}