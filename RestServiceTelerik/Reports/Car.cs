using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceTelerik.Reports
{
    public class Car
    {
        string manufacturer;
        string model;
        string year;
        DateTime salesStart;
        decimal salesCredit;
        double? salesDebit;
        string imageUrl;
        ArrayList availableColor;
        string promotion;
        bool available;
        System.Collections.Generic.List<string> description;

        public Car(string manufacturer, string model, string year, string imageUrl, string[] availableColor, string promotion, decimal salesCredit, double? salesDebit, DateTime salesStart, bool available, System.Collections.Generic.List<string> description)
        {
            this.manufacturer = manufacturer;
            this.model = model;
            this.year = year;
            this.imageUrl = imageUrl;
            this.AvailableColor = new ArrayList(availableColor);
            this.Promotion = promotion;
            this.available = available;
            this.salesCredit = salesCredit;
            this.salesDebit = salesDebit;
            this.salesStart = salesStart;
            this.description = description;
        }

        public DateTime SalesStart
        {
            get { return this.salesStart; }
            set { this.salesStart = value; }
        }

        public System.Collections.Generic.List<string> Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }

        }

        public bool Available
        {
            get { return this.available; }
            set { this.available = value; }
        }

        public decimal SalesCredit
        {
            get { return this.salesCredit; }
            set { this.salesCredit = value; }
        }

        public double? SalesDebit
        {
            get { return this.salesDebit; }
            set { this.salesDebit = value; }
        }

        public string Model
        {
            get { return this.model; }
            set { this.model = value; }
        }

        public string Manufacturer
        {
            get { return this.manufacturer; }
            set { this.manufacturer = value; }
        }

        public string Year
        {
            get { return this.year; }
            set { this.year = value; }
        }

        public string ImageUrl
        {
            get { return this.imageUrl; }
            set { this.imageUrl = value; }
        }

        public ArrayList AvailableColor
        {
            get { return this.availableColor; }
            set { this.availableColor = value; }
        }

        public string Promotion
        {
            get { return this.promotion; }
            set { this.promotion = value; }
        }
    }
}