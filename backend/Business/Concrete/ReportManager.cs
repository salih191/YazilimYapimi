using System;
using System.Collections.Generic;
using System.IO;
using Business.Abstract;
using ChoETL;
using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Concrete
{
    public class ReportManager:IReportService
    {
        private IOrderService _orderService;
        private ICategoryService _categoryService;

        public ReportManager(IOrderService orderService, ICategoryService categoryService)
        {
            _orderService = orderService;
            _categoryService = categoryService;
        }

        public IResult Report(ReportInfo reportInfo)
        {
            var result = a(reportInfo);
            string str = "";
            try
            {
                File.Delete($"wwwroot/csv/{reportInfo.UserId}.csv");
                using (var w = new ChoCSVWriter<Report>($"wwwroot/csv/{reportInfo.UserId}.csv").WithFirstLineHeader())
                {
                    w.Write(result.Data);
                    w.Dispose();
                }

                return new SuccessResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine(result.Data.GetXml());
            return new ErrorResult();
        }

        private IDataResult<List<Report>> a(ReportInfo reportInfo)
        {
            var result = _orderService.GetByReport(reportInfo).Data;
            var report = new List<Report>();
            foreach (var order in result)
            {
                report.Add(new Report{AlimTutari = order.UnitPrice,Miktar = order.Quantity,Tarih = order.OrderDate.ToShortDateString(),UrunTipi = _categoryService.GetById(order.CategoryId).Data.Name });
            }
            return new SuccessDataResult<List<Report>>(report);
        }
    }
}