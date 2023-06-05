using log4net;
using log4net.Config;
using QuoteLineBot1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Telegram.Bot;

namespace CasinoPaymentUsdtService
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //private static readonly string PurgeToken = "5165944123:AAGgk-retaEQtBLwWI5nvQVDlVs38Y89JsM";
        private static readonly string PurgeToken = "5729751481:AAE3RpJzg4vr3xAQ85zU4G4pz_IaHGUhqqY";
        private static readonly TelegramBotClient PurgeBot = new TelegramBotClient(PurgeToken);

        static string randy = "1009737887";//
        static string group = "-1001873872614";//

        static void Main(string[] args)
        {
            InitialConfiguration();
            log.Info("start console");

            var initSend = PurgeBot.SendTextMessageAsync(group, "娛樂城 USDT 代收付 init");
            initSend.Wait();

            //string addressBalanceNowStr = getTronscan.Get("THL1P3gA9xq8AZ6maufU68K16Rs1xbPyqt");

            while (true)
            {
                try
                {
                    Utility.LogAndConsole("Thread_Payment");

                    // Hide
                    //ShowWindow(handle, SW_HIDE);

                    string dateTimeMMdd = DateTime.UtcNow.AddHours(8).ToString("MM/dd");

                    //1分鐘查詢1次 (支付)
                    //查詢 _payment_req 狀態 為 pending 的資料
                    List<PaymentReqObj> paymentPendingReqList = new List<PaymentReqObj>();
                    int tryCount = 0;
                    while (true)
                    {
                        try
                        {
                            tryCount++;
                            paymentPendingReqList = PaymentTask.SelectPendingReq();
                            break;
                        }
                        catch (Exception ex1)
                        {
                            System.Threading.Thread.Sleep(60 * 1000);
                            if (tryCount >= 3)
                            {
                                string reTxt = "娛樂城 USDT PaymentTask.SelectPendingReq 連接失敗已超過 3次以上";
                                var task = PurgeBot.SendTextMessageAsync(group, reTxt);
                                task.Wait();
                                Utility.LogAndConsole("CoinShaBot", $"Bot.SendTextMessageAsync({group}, {reTxt}");
                                throw;
                            }
                            continue;
                        }
                    }



                    foreach (var paymentPendingReq in paymentPendingReqList)
                    {
                        // 取得當下鏈上餘額
                        decimal addressBalanceNow = 0;
                        string addressBalanceNowStr = getTronscan.Get(paymentPendingReq.address);
                        bool isaddressBalanceNowDecimal = decimal.TryParse(addressBalanceNowStr, out addressBalanceNow);
                        addressBalanceNow = addressBalanceNow / 1000000.0M;
                        Utility.LogAndConsole($"(Thread_Payment) UserId: {paymentPendingReq.user_id}，Address: {paymentPendingReq.address}，addressBalanceNow: {addressBalanceNow}");

                        if (!isaddressBalanceNowDecimal)
                        {
                            // 通知 group return
                            string reTxt = $"1分鐘查詢1次 (支付) 發生預期外的狀況";
                            PurgeBot.SendTextMessageAsync(group, reTxt);
                            Utility.LogAndConsole("CoinShaBot", $"Bot.SendTextMessageAsync({group}, {reTxt}");
                            return;
                        }

                        // 查詢該地址的當下餘額，是否有大於該筆資料的初始錢包金額 + 儲值金額
                        decimal paymentAmount = 0;
                        bool isPaymentAmountDecimal = decimal.TryParse(paymentPendingReq.amount, out paymentAmount);
                        decimal startBalance = 0;
                        bool isPaymentBalanceDecimal = decimal.TryParse(paymentPendingReq.start_balance, out startBalance);
                        Utility.LogAndConsole($"(Thread_Payment)  paymentAmount: {paymentAmount} ， startBalance: {startBalance}");

                        if (addressBalanceNow >= (paymentAmount + startBalance))//支付成功
                        {
                            //更新支付任務資料狀態為 done & 完成後餘額
                            PaymentTask.Update_payment_req_Status(paymentPendingReq.id, "done");
                            Utility.LogAndConsole($"(Thread_Payment)  PaymentTask.Update_payment_req_Status {paymentPendingReq.id}， done");
                            PaymentTask.Update_payment_req_end_balance(paymentPendingReq.id, addressBalanceNow.ToString());
                            Utility.LogAndConsole($"(Thread_Payment)  PaymentTask.Update_payment_req_end_balance {paymentPendingReq.id}， {addressBalanceNow}");

                            //主動發起端口通知給對方
                            //TODO

                            // 通知該群組支付成功
                            string reTxt = $"娛樂城 {paymentPendingReq.user_id} 金額 {paymentPendingReq.amount} 支付成功" + Environment.NewLine
                                    + paymentPendingReq.address + Environment.NewLine + paymentPendingReq.id;
                            PurgeBot.SendTextMessageAsync(group, reTxt);
                            Utility.LogAndConsole("CoinShaBot", $"Bot.SendTextMessageAsync({group}, {reTxt}");
                        }
                        else//尚未支付成功
                        {
                            DateTime dtNow = DateTime.UtcNow.AddHours(8);
                            DateTime req_date = DateTime.Parse(paymentPendingReq.req_date);

                            //如果現在時間大於付款期限，則代表超過時間未支付
                            if (dtNow > req_date.AddMinutes(33))//寬限三分鐘
                            {
                                //更新支付任務資料狀態為 expired & 完成後餘額
                                PaymentTask.Update_payment_req_Status(paymentPendingReq.id, "expired");
                                Utility.LogAndConsole($"(Thread_Payment)  PaymentTask.Update_payment_req_Status {paymentPendingReq.id}， {"expired"}");
                                PaymentTask.Update_payment_req_end_balance(paymentPendingReq.id, addressBalanceNow.ToString());
                                Utility.LogAndConsole($"(Thread_Payment)  PaymentTask.Update_payment_req_end_balance {paymentPendingReq.id}， {addressBalanceNow}");

                                // 通知該群組超時支付
                                string reTxt = $"娛樂城 {paymentPendingReq.user_id} 金額 {paymentPendingReq.amount} 超時支付" + Environment.NewLine
                                    + paymentPendingReq.address + Environment.NewLine + paymentPendingReq.id;
                                PurgeBot.SendTextMessageAsync(group, reTxt);
                                Utility.LogAndConsole("CoinShaBot", $"Bot.SendTextMessageAsync({group}, {reTxt}");
                            }
                        }
                        Utility.LogAndConsole($"(Thread_Payment)  ");
                        System.Threading.Thread.Sleep(300);
                    }
                }
                catch (Exception ex)
                {
                    string reTxt = "PaymentTask Error";
                    var task = PurgeBot.SendTextMessageAsync(group, reTxt);
                    task.Wait();
                    log.Error(ex.ToString());
                    throw;
                }

                System.Threading.Thread.Sleep(60 * 1000);
            }



        }

        private static void InitialConfiguration()
        {
            var directory = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(directory, "log4net.config")));
        }
    }
}
