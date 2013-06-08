using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Frost.SatPwr
{
    public partial class ToggleButton : Button
    {
        protected bool toggleState;
        protected string truePrefixText;
        protected string falsePrefixText;
        protected string suffixText;
        public bool ToggleState
        {
            get
            {
                return toggleState;
            }
            set
            {
                toggleState = value;
                UpdateText();
            }
        }

        public string TruePrefixText
        {
            get
            {
                return truePrefixText;
            }
            set
            {
                truePrefixText = value;
                UpdateText();
            }
        }

        public string FalsePrefixText
        {
            get
            {
                return falsePrefixText;
            }
            set
            {
                falsePrefixText = value;
                UpdateText();
            }
        }

        public string SuffixText
        {
            get
            {
                return suffixText;
            }
            set
            {
                suffixText = value;
                UpdateText();
            }
        }

        public ToggleButton()
        {
            InitializeComponent();
            toggleState = false;
            truePrefixText = "TruePrefixText";
            falsePrefixText = "FalsePrefixText";
            suffixText = "SuffixText";
            UpdateText();
        }

        protected override void OnClick(EventArgs e)
        {
            toggleState = !toggleState;
            UpdateText();
            base.OnClick(e);
        }

        public void UpdateText()
        {
            this.Text = (toggleState ? TruePrefixText : FalsePrefixText) + suffixText;
        }
    }
}
