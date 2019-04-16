using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceTelerik.Reports
{
    using System.Collections.Generic;
    using System;

    public class Cars : List<Car>
    {
        public Cars()
        {
            Car car;

            for (int i = 20; i > 0; --i)
            {
                car = new Car("Honda", "NSX GT", "2003", "http://www.telerik.com/images/reporting/cars/NSXGT_7.jpg"
                    , new string[] { "Red", "Blue", "Black", "Yellow", "White" }, "TV", new Decimal(123452.02), 155.55, new System.DateTime(2006, 1, 18), true, new System.Collections.Generic.List<string> { "description1", "description2" });
                this.Add(car);

                car = new Car("Honda", "NSX GT", "2004", "http://www.telerik.com/images/reporting/cars/NSXGT_7.jpg"
    , new string[] { "Red", "Blue", "Black", "Yellow", "White" }, "TV", new Decimal(123452.02), 155.55, new System.DateTime(2006, 1, 18), true, new System.Collections.Generic.List<string> { "description1", "description2" });
                this.Add(car);

                car = new Car("Honda", "Civic", "2005", "http://www.telerik.com/images/reporting/cars/NSXGT_7.jpg"
                    , new string[] { "Red", "Blue", "Black", "Yellow", "White" }, "TV", new Decimal(153452.33), 195.6, new System.DateTime(2006, 5, 28), true, new System.Collections.Generic.List<string> { "description1" });
                this.Add(car);

                car = new Car("Nissan", "370 Z", "2006", "http://www.telerik.com/images/reporting/cars/EVLR34_1.jpg"
                    , new string[] { "Blue", "White" }, "TV", new Decimal(123452.2), 0.00, new System.DateTime(2005, 3, 18), true, new System.Collections.Generic.List<string> { "description1", "description2", "description3" });
                this.Add(car);

                car = new Car("Nissan", "Qashqai ", "2007", "http://www.telerik.com/images/reporting/cars/EVLR34_1.jpg"
                    , new string[] { "Blue", "White" }, "TV", new Decimal(122252.2), 111.000, new System.DateTime(2005, 3, 10), true, new System.Collections.Generic.List<string> { "description1" });
                this.Add(car);

                car = new Car("Nissan", "Qashqai ", "2008", "http://www.telerik.com/images/reporting/cars/EVLR34_1.jpg"
    , new string[] { "Blue", "White" }, "TV", new Decimal(122252.2), 111.000, new System.DateTime(2005, 3, 10), true, new System.Collections.Generic.List<string> { "description1" });
                this.Add(car);

                car = new Car("Mazda", "MX-5 SE", "2009", "http://www.telerik.com/images/reporting/cars/MX5_1.jpg"
                    , new string[] { "Blue" }, "TV", new Decimal(83452.69), 130205.23, new System.DateTime(2004, 3, 11), false, new System.Collections.Generic.List<string> { "description1", "description2" });
                this.Add(car);

                car = new Car("Audi", "R8", "2010", "http://www.telerik.com/images/reporting/cars/S4_3.jpg"
                    , new string[] { "Red", "Black", "White" }, "TV", new Decimal(88452.22), 160555.22, new System.DateTime(2006, 9, 13), true, new System.Collections.Generic.List<string> { "description1", "description2" });
                this.Add(car);

                car = new Car("Audi", "R8", "2010", "http://www.telerik.com/images/reporting/cars/S4_3.jpg"
    , new string[] { "Red", "Black", "White" }, "TV", new Decimal(88452.22), 160555.22, new System.DateTime(2006, 9, 13), true, new System.Collections.Generic.List<string> { "description1", "description2" });
                this.Add(car);

            }
        }
    }
}