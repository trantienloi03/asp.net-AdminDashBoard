﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020484.DomainModels
{
    public class ViewOrder
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderTime { get; set; }
        public string DeliveryProvince { get; set; } = "";
        public string DeliveryAddress { get; set; } = "";
        public int Status { get; set; }
        /// <summary>
        /// Mô tả trạng thái đơn hàng dựa trên mã trạng thái
        /// </summary>
        public string StatusDescription
        {
            get
            {
                switch (Status)
                {
                    case Constants.ORDER_INIT:
                        return "Đơn hàng mới. Đang chờ duyệt";
                    case Constants.ORDER_ACCEPTED:
                        return "Đơn đã chấp nhận. Đang chờ chuyển hàng";
                    case Constants.ORDER_SHIPPING:
                        return "Đơn hàng đang được giao";
                    case Constants.ORDER_FINISHED:
                        return "Đơn hàng đã hoàn tất";
                    case Constants.ORDER_CANCEL:
                        return "Đơn hàng đã bị hủy";
                    case Constants.ORDER_REJECTED:
                        return "Đơn hàng bị từ chối";
                    default:
                        return "";
                }
            }
        }
    }
}

