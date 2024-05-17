using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE.MODEL
{
    /// <summary>
    /// 计划的周期
    /// </summary>
    public enum enmCycle
    {
        日 = 1,
        月 = 2,
        季 = 3,
        半年 = 4,
        年 = 5
    }

    public enum enmWBPlanStatus
    {
        作废 = -1,
        计划编制 = 0,
        待审核 = 1,
        审核成功 = 2,
        审核不通过 = 3
    }

    /// <summary>
    /// 维护工单的状态
    /// 工单记录补充
    /// </summary>
    public enum enmWBOrderStatus
    {
        待执行 = 1,
        执行成功 = 2
    }

    /// <summary>
    /// 维护  工单进度表
    /// </summary>
    public enum WB_Order_Status
    {
        待接单 = 0,
        巡查中 = 1,
        待审核 = 2,
        已结束 = 3
    }

    /// <summary>
    /// 维修工单的状态
    /// </summary>
    public enum enmWXOrderStatus
    {
        工单制定 = -1,
        待接单 = 0,
        维修中 = 1,
        待审批 = 2,
        已结束 = 3,
    }

    /// <summary>
    /// 日常巡检工单状态例外巡检业务实施全过程
    /// </summary>
    public enum enmBusinessStatus
    {
        草稿=-1,
        待接单 = 0,
        执行中 = 1,
        待审批 = 2,
        已结束 = 3
    }

    /// <summary>
    /// 问题解决状态
    /// </summary>
    public enum enmDealStatus
    {
        已解决 = 1,
        未解决 = 2,
        临时解决 = 3,  //临时解决需要定期重新处理该工单
    }


    /// <summary>
    /// 路由配置的状态
    /// </summary>
    public enum enmRouteConfigStatus
    {
        已删除 = 0,
        草稿 = 1,
        已启用 = 2
    }
    /// <summary>
    /// 维修工单来源
    /// </summary>
    public enum enmOrderType
    {
        日常巡检 = 1,
        预防性维护 = 2,
        隐患问题 = 3,
        自定义 = 4
    }
    /// <summary>
    /// 巡检计划配置状态
    /// </summary>
    public enum enmConfigPatrolStatus
    {
        草稿 = 0,
        待审核 = 1,
        审批通过 = 2,
        审批不通过 = 3,
    }

    /// <summary>
    /// SPC_Space 类型 卫生打扫区域类型
    /// </summary>
    public enum enumSpaceType
    {
        公共区域 = 1,
        设备区域 = 2
    }
    /// <summary>
    /// 维修等级
    /// </summary>
    public enum enmGrade
    {
        高 = 1,
        中 = 2,
        低 = 3,
    }

    #region 文档管理 

    /// <summary>
    /// 文档管理绑定类别
    /// </summary>
    public enum enumDocBindingType
    {
        类型绑定 = 2,
        标识绑定 = 3,
        不绑定 = 1
    }

    /// <summary>
    /// 文档管理类别
    /// </summary>
    public enum enumDocTreeType
    {
        管理制度 = 1,
        流程指引 = 2,
        SOP_巡检指引 = 3,
        MOP_维护指引 = 4,
        EOP_应急预案 = 5,
        竣工图纸 = 6,
        产品手册 = 7,
        周_月_年报管理 = 8,
        合同管理 = 9
    }

    #endregion

    /// <summary>
    /// 工单类型
    /// </summary>
    public enum enmOrderNameType
    {
        巡检 = 1,
        维护 = 2,
        维修 = 3,
        隐患 = 4,
        卫生 = 5,
        例外 = 6
    }

    public enum enmDocType
    {
        普通资料 = 1,
        预案资料 = 2
    }
    /// <summary>
    /// 用户密码难易程度
    /// </summary>
    public enum passwordLevel
    {
        简单  =1,
        一般 =2,
        复杂=3

    }
}
