// --------------------------------------------------------------------------
// <copyright file="EntityBase.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Models
{
    using System;
    using Fanex.NAP.Framework.Domain;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    /// <typeparam name="TName">The type of the name.</typeparam>
    [Serializable]
    public class EntityBase<TId, TName> : IEntity<TId, TName>
    {
        public TId Id
        {
            get;
            set;
        }

        public TName Name
        {
            get;
            set;
        }
    }
}