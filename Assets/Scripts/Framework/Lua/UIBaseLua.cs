﻿using System;
using Lite.Framework.Base;
using XLua;

namespace Lite.Framework.Lua
{
    public class UIBaseLua : UIBase
    {
        private readonly LuaTable Entity_;
        private Action<LuaTable> OnOpen_;
        private Action<LuaTable> OnClose_;
        private Action<LuaTable> OnShow_;
        private Action<LuaTable> OnHide_;
        private Action<LuaTable, float> OnTick_;

        public UIBaseLua(LuaTable LuaEntity)
            : base()
        {
            Entity_ = LuaEntity;
            Entity_.SetInPath("_CSEntity_", this);
            OnOpen_ = LuaEntity.GetInPath<Action<LuaTable>>("OnOpen");
            OnClose_ = LuaEntity.GetInPath<Action<LuaTable>>("OnClose");
            OnShow_ = LuaEntity.GetInPath<Action<LuaTable>>("OnShow");
            OnHide_ = LuaEntity.GetInPath<Action<LuaTable>>("OnHide");
            OnTick_ = LuaEntity.GetInPath<Action<LuaTable, float>>("OnTick");

            if (LuaEntity.Exists<string>("Name"))
            {
                Name = LuaEntity.GetInPath<string>("Name");
            }

            if (LuaEntity.Exists<UIDepthMode>("DepthMode"))
            {
                DepthMode = LuaEntity.GetInPath<UIDepthMode>("DepthMode");
            }

            if (LuaEntity.Exists<int>("Depth"))
            {
                Depth = LuaEntity.GetInPath<int>("Depth");
            }
        }

        protected override void OnOpen(params object[] Params)
        {
            OnOpen_?.Invoke(Entity_);
        }

        protected override void OnClose()
        {
            OnClose_?.Invoke(Entity_);

            OnOpen_ = null;
            OnClose_ = null;
            OnShow_ = null;
            OnHide_ = null;
            OnTick_ = null;
        }

        protected override void OnShow()
        {
            OnShow_?.Invoke(Entity_);
        }

        protected override void OnHide()
        {
            OnHide_?.Invoke(Entity_);
        }

        protected override void OnTick(float DeltaTime)
        {
            OnTick_?.Invoke(Entity_, DeltaTime);
        }
    }
}