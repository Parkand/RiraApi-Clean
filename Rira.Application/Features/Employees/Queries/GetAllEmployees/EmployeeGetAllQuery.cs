using MediatR;
using Rira.Application.Common;
using Rira.Application.DTOs;
using System.Collections.Generic;

namespace Rira.Application.Features.Employees.Queries.GetAllEmployees
{
    /// <summary>
    /// 🎯 Query اصلی برای بازیابی لیست تمام کارمندان سیستم ریرا
    /// ------------------------------------------------------------
    /// این کلاس بخشی از معماری CQS است که عملیات *خواندن داده* را مشخص می‌کند.
    /// 
    /// 💡 برخلاف Commandها، Queryها تنها مسئول دریافت اطلاعات هستند
    /// و هیچ تغییری در وضعیت دیتابیس ایجاد نمی‌کنند.
    ///
    /// خروجی استاندارد از نوع ResponseModel<List<EmployeeDTO>> است.
    /// </summary>
    public class EmployeeGetAllQuery : IRequest<ResponseModel<List<EmployeeDTO>>>
    {
        /*
         🔹 نکته: فعلاً این Query هیچ پارامتری ندارد و تمام کارمندان را بازمی‌گرداند.
             اما در آینده می‌توان:
                - فیلتر‌های اختیاری (مثلاً بر اساس فعال بودن یا سمت)
                - صفحه‌بندی (Page, PageSize)
             به آن اضافه کرد.
        */
    }
}
