﻿using Flurl.Http;
using ShikkhanobishStudentApp.Model;
using ShikkhanobishStudentApp.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShikkhanobishStudentApp.ViewModel
{
    public class StudentProfile : BaseViewModel, INotifyPropertyChanged
    {
        #region Methods
        int changeFlag = 0;
        Student thisStudent { get; set; }
        public StudentProfile()
        {
            PopulatePaymentList();
            PopulateTuitionList();
            thisStudent = StaticPageToPassData.thisStudentInfo;
            name = StaticPageToPassData.thisStudentInfo.name;
            phonenumber = StaticPageToPassData.thisStudentInfo.phonenumber;
            coinSpent = "" + StaticPageToPassData.thisStudentInfo.totalSpent;
            totalTuition = "" + StaticPageToPassData.thisStudentInfo.totalTuitionTime;
            tuitionHisChanged = true;
            paymentHistoryChaged = false;
            hisVisibility = true;
            payVisiblity = false;
            popUpVisibility = false;
            popBtnEnabled = false;
            popBtnTxt = "Change";

        }
        public ICommand closeWindow =>
            new Command(() =>
            {
                Application.Current.MainPage.Navigation.PopModalAsync();
            });

        private void changepass()
        {
            hasErrorF = false;
            hasErrorS = false;
            errorTxtF = "";
            errorTxtS = "";
            changeFlag = 1;
            popUpVisibility = true;
            popupTitle = "Change Password";
            popuptxtFieldPlcHolder = "New Password";
            popBtnEnabled = false;
        }
        private void changepn()
        {
            hasErrorF = false;
            hasErrorS = false;
            errorTxtF = "";
            errorTxtS = "";
            changeFlag = 2;
            popUpVisibility = true;
            popupTitle = "Change Phone Number";
            popuptxtFieldPlcHolder = "New Phone Number";
            popBtnEnabled = false;
        }
        private void changeNmae()
        {
            hasErrorF = false;
            hasErrorS = false;
            errorTxtF = "";
            errorTxtS = "";
            changeFlag = 3;
            popUpVisibility = true;
            popupTitle = "Change Name";
            popuptxtFieldPlcHolder = "New Name";
            popBtnEnabled = false;
        }
        private void PerformpopOut()
        {
            popUpVisibility = false;
            popupTitle = "";
            popuptxtFieldPlcHolder = "";
            hasErrorS = false;
            errorTxtS = "";
            hasErrorF = false;
            errorTxtF = "";
            if (passtext != null)
            {
                passtext = "";
            }
            if (newInfoText != null)
            {
                newInfoText = "";

            }
        }
        public async Task PopulateTuitionList()
        {
            List<StudentTuitionHistory> tuitionList = await "https://api.shikkhanobish.com/api/ShikkhanobishLogin/getStudentTuitionHistoryWithID".PostUrlEncodedAsync(new { studentID = StaticPageToPassData.thisStudentInfo.studentID})
        .ReceiveJson<List<StudentTuitionHistory>>();
            List<StudentTuitionHistory> SortedList = new List<StudentTuitionHistory>();
            SortedList = tuitionList.OrderBy(x => x.date).ToList();
            SortedList.Reverse();
            tuiListItemSource = SortedList;           
        }
        public async Task PopulatePaymentList()
        {
            IEnumerable<StudentPaymentHistory> payList = await "https://api.shikkhanobish.com/api/ShikkhanobishLogin/getStudentPaymentHistoryWithID".PostUrlEncodedAsync(new { studentID = StaticPageToPassData.thisStudentInfo.studentID })
        .ReceiveJson<IEnumerable<StudentPaymentHistory>>();
            List<StudentPaymentHistory> SortedList = new List<StudentPaymentHistory>();
            SortedList = payList.OrderBy(x => x.date).ToList();
            SortedList.Reverse();
            paymentList = SortedList;
        }
        public void Check()
        {
            bool error = false;
            if(passtext != null)
            {
                if ( passtext != StaticPageToPassData.thisStudentInfo.password)
                {
                    hasErrorS = true;
                    errorTxtS = "This is not your current password";

                    popBtnEnabled = false;
                    error = true;
                }
                else
                {
                    hasErrorS = false;
                    errorTxtS = "";
                }
            }
            else
            {
                error = true;
            }
            if(newInfoText != null)
            {
                bool isdigit = newInfoText.Any(char.IsDigit);
                if (changeFlag == 1 && (!isdigit || newInfoText.Length < 6))
                {
                    hasErrorF = true;
                    errorTxtF = "Minimum 6 character with 1 digit";
                    popBtnEnabled = false;

                    error = true;
                }
                else
                {
                    hasErrorF = false;
                    errorTxtF = "";
                }
            }
            else
            {
                error = true;
            }

            if (changeFlag == 2 && newInfoText.Length != 11)
            {
                hasErrorF = true;
                errorTxtF = "Invalid Phone Number";
                popBtnEnabled = false;
                
                error = true;
            }
            if (!error)
            {
                popBtnEnabled = true;
                hasErrorF = false;
                hasErrorS = false;
                errorTxtF = "";
                errorTxtS = "";
            }
            
        }
        public ICommand changeCommand =>
              new Command( async() =>
              {
                  popBtnEnabled = false;
                  popBtnTxt = "Wait..";
                  if (changeFlag == 1)
                  {
                      Response regRes = await "https://api.shikkhanobish.com/api/ShikkhanobishLogin/updateStudent"
                .PostUrlEncodedAsync(new
                {
                    studentID = thisStudent.studentID,
                    phonenumber = thisStudent.phonenumber,
                    password = newInfoText,
                    totalSpent = thisStudent.totalSpent,
                    totalTuitionTime = thisStudent.totalTuitionTime,
                    coin = thisStudent.coin,
                    freemin = thisStudent.freemin,
                    city = thisStudent.city,
                    name = thisStudent.name,
                    institutionName = "none"
                })
                .ReceiveJson<Response>();
                  }

                  if (changeFlag == 2)
                  {
                      Response regRes = await "https://api.shikkhanobish.com/api/ShikkhanobishLogin/updateStudent"
               .PostUrlEncodedAsync(new
               {
                   studentID = thisStudent.studentID,
                   phonenumber = newInfoText,
                   password = thisStudent.password,
                   totalSpent = thisStudent.totalSpent,
                   totalTuitionTime = thisStudent.totalTuitionTime,
                   coin = thisStudent.coin,
                   freemin = thisStudent.freemin,
                   city = thisStudent.city,
                   name = thisStudent.name,
                   institutionName = "none"
               })
               .ReceiveJson<Response>();
                  }

                  if (changeFlag == 3)
                  {
                      Response regRes = await "https://api.shikkhanobish.com/api/ShikkhanobishLogin/updateStudent"
               .PostUrlEncodedAsync(new
               {
                   studentID = thisStudent.studentID,
                   phonenumber = thisStudent.phonenumber,
                   password = thisStudent.password,
                   totalSpent = thisStudent.totalSpent,
                   totalTuitionTime = thisStudent.totalTuitionTime,
                   coin = thisStudent.coin,
                   freemin = thisStudent.freemin,
                   city = thisStudent.city,
                   name = newInfoText,
                   institutionName = "none"
               })
               .ReceiveJson<Response>();
                  }

                  if(changeFlag == 1)
                  {
                      StaticPageToPassData.thisStudentInfo = await "https://api.shikkhanobish.com/api/ShikkhanobishLogin/LoginStudent".PostUrlEncodedAsync(new { phonenumber = StaticPageToPassData.thisStPh, password = passtext })
         .ReceiveJson<Student>();
                  }
                  else if(changeFlag == 2)
                  {
                      StaticPageToPassData.thisStudentInfo = await "https://api.shikkhanobish.com/api/ShikkhanobishLogin/LoginStudent".PostUrlEncodedAsync(new { phonenumber = newInfoText, password = StaticPageToPassData.thisstPass })
         .ReceiveJson<Student>();
                  }
                  else if (changeFlag == 3)
                  {
                      StaticPageToPassData.thisStudentInfo = await "https://api.shikkhanobish.com/api/ShikkhanobishLogin/LoginStudent".PostUrlEncodedAsync(new { phonenumber = StaticPageToPassData.thisStPh, password = StaticPageToPassData.thisstPass })
         .ReceiveJson<Student>();
                  }
                  name = StaticPageToPassData.thisStudentInfo.name;
                  phonenumber = StaticPageToPassData.thisStudentInfo.phonenumber;
                  popUpVisibility = false;
                  popupTitle = "";
                  popBtnEnabled = true;
                  popBtnTxt = "Change";
              });
        #endregion//dfsdfs

        #region Binding
        private string name1;

        public string name { get => name1; set => SetProperty(ref name1, value); }

        private string phonenumber1;

        public string phonenumber { get => phonenumber1; set => SetProperty(ref phonenumber1, value); }

        private Command changepnCommand1;

        public ICommand changepnCommand
        {
            get
            {
                if (changepnCommand1 == null)
                {
                    changepnCommand1 = new Command(changepn);
                }

                return changepnCommand1;
            }
        }

        

        private Command changepassCommand1;

        public ICommand changepassCommand
        {
            get
            {
                if (changepassCommand1 == null)
                {
                    changepassCommand1 = new Command(changepass);
                }

                return changepassCommand1;
            }
        }

        private string coinSpent1;

        public string coinSpent { get => coinSpent1; set => SetProperty(ref coinSpent1, value); }

        private string totalTuition1;

        public string totalTuition { get => totalTuition1; set => SetProperty(ref totalTuition1, value); }

        private bool tuitionHisChanged1;

        public bool tuitionHisChanged { get { return tuitionHisChanged1; } set { tuitionHisChanged1 = value;
                if(tuitionHisChanged == true)
                {
                    paymentHistoryChaged = false;
                    hisVisibility = true;
                    payVisiblity = false;
                }
                
                OnPropertyChanged(); } }

        private bool paymentHistoryChaged1;

        public bool paymentHistoryChaged { get { return paymentHistoryChaged1; } set { paymentHistoryChaged1 = value;
                if (paymentHistoryChaged == true)
                {
                    tuitionHisChanged = false;
                    hisVisibility = false;
                    payVisiblity = true;
                }
                OnPropertyChanged(); } }

        private Command changeNmaeCommand1;

        public ICommand changeNmaeCommand
        {
            get
            {
                if (changeNmaeCommand1 == null)
                {
                    changeNmaeCommand1 = new Command(changeNmae);
                }

                return changeNmaeCommand1;
            }
        }

       

        private bool payVisiblity1;

        public bool payVisiblity { get => payVisiblity1; set => SetProperty(ref payVisiblity1, value); }

        private bool hisVisibility1;

        public bool hisVisibility { get => hisVisibility1; set => SetProperty(ref hisVisibility1, value); }

        private bool popUpVisibility1;

        public bool popUpVisibility { get => popUpVisibility1; set => SetProperty(ref popUpVisibility1, value); }

        private string popupTitle1;

        public string popupTitle { get => popupTitle1; set => SetProperty(ref popupTitle1, value); }

        private string popuptxtFieldPlcHolder1;

        public string popuptxtFieldPlcHolder { get => popuptxtFieldPlcHolder1; set => SetProperty(ref popuptxtFieldPlcHolder1, value); }

        private Command popOut1;

        public ICommand popOut
        {
            get
            {
                if (popOut1 == null)
                {
                    popOut1 = new Command(PerformpopOut);
                }

                return popOut1;
            }
        }

        private bool popBtnEnabled1;

        public bool popBtnEnabled { get => popBtnEnabled1; set => SetProperty(ref popBtnEnabled1, value); }

        private string newInfoText1;

        public string newInfoText { get { return newInfoText1; } set { newInfoText1 = value; Check(); OnPropertyChanged(); } }

        private string passtext1;

        public string passtext { get { return passtext1; } set { passtext1 = value; Check(); OnPropertyChanged(); } }


        
        

        private string errorTxtF1;

        public string errorTxtF { get => errorTxtF1; set => SetProperty(ref errorTxtF1, value); }

        private string errorTxtS1;

        public string errorTxtS { get => errorTxtS1; set => SetProperty(ref errorTxtS1, value); }

        private bool hasError1;

        public bool hasError { get => hasError1; set => SetProperty(ref hasError1, value); }

        private bool hasErrorF1;

        public bool hasErrorF { get => hasErrorF1; set => SetProperty(ref hasErrorF1, value); }

        private bool hasErrorS1;

        public bool hasErrorS { get => hasErrorS1; set => SetProperty(ref hasErrorS1, value); }

        private string popBtnTxt1;

        public string popBtnTxt { get => popBtnTxt1; set => SetProperty(ref popBtnTxt1, value); }

        private List<StudentTuitionHistory> tuiListItemSource1;

        public List<StudentTuitionHistory> tuiListItemSource { get => tuiListItemSource1; set => SetProperty(ref tuiListItemSource1, value); }

        private System.Collections.IEnumerable paymentList1;

        public System.Collections.IEnumerable paymentList { get => paymentList1; set => SetProperty(ref paymentList1, value); }



        #endregion
    }
}
