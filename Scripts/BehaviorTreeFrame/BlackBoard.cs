
using System.Collections.Generic;


namespace BehaviorTreeFrame
{
    /// <summary>
    /// 黑板，用做数据共享
    /// </summary>
    public class BlackBoard<Entity>
    {

        public int[][] Map;
        public int sizeH;//高、行数
        public int sizeW;//宽、列数
        public  int Timeout=40;//系统刷新频率（多少毫秒一次）
        private static BlackBoard<Entity> blackBoard;//黑板实例
        public  int time = 0;//游戏时间
        public List<Entity> entities = new List<Entity>();//实体列表
        //---------------------------------------------//
        public int enablePhysical = 50;//可以活动的体力值
        public int Friction = 10;//摩擦力
        protected BlackBoard()
        {

        }
        
        /// <summary>
        /// 创建地图二维数组
        /// </summary>
        /// <param name="sizeH"></param>
        /// <param name="sizeW"></param>
        public void createMap(int sizeH,int sizeW)
        {
            this.sizeH = sizeH;
            this.sizeW = sizeW;
            Map = new int[sizeH][];
            for(int i = 0; i < sizeH; i++)
            {
                Map[i] = new int[sizeW];
            }
        }
         
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static BlackBoard<Entity> getInstance()
        {
            if (blackBoard == null)
            {
                blackBoard = new BlackBoard<Entity>();
            }
            return blackBoard;
        }

    }
}
