// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*++
                                                                         
    Abstract:
        This file contains the definition of a class that defines
        the common functionality required to serialize a PageContent.                                                                         
--*/
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Security;
using System.ComponentModel.Design.Serialization;
using System.Windows.Xps.Packaging;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;

namespace System.Windows.Xps.Serialization
{
    /// <summary>
    /// Class defining common functionality required to
    /// serialize a ReachPageContentSerializer.
    /// </summary>
    internal class NgcPageContentSerializerAsync :
                   NGCSerializerAsync
    {
        #region Constructor

        /// <summary>
        /// Constructor for class ReachPageContentSerializer
        /// </summary>
        /// <param name="manager">
        /// The serialization manager, the services of which are
        /// used later in the serialization process of the type.
        /// </param>
        public
        NgcPageContentSerializerAsync(
            PackageSerializationManager   manager
            ):
        base(manager)
        {
            
        }

        #endregion Constructor
        
        #region Public Methods
        
        /// <summary>
        ///
        /// </summary>
        public
        override
        void
        AsyncOperation(
            NGCSerializerContext context
            )
        {
            if(context == null)
            {

            }
           
            switch (context.Action) 
            {
                case SerializerAction.serializePage:
                {
                    SerializePage(context.ObjectContext);
                    break;
                }
                
                default:
                {
                    base.AsyncOperation(context);
                    break;
                }
            }
        }
        

        #endregion

        
        #region Internal Methods

        /// <summary>
        /// The method is called once the object data is discovered at that 
        /// point of the serialization process.
        /// </summary>
        /// <param name="serializableObjectContext">
        /// The context of the object to be serialized at this time.
        /// </param>
        internal
        override
        void
        PersistObjectData(
            SerializableObjectContext   serializableObjectContext
            )
        {
            if(serializableObjectContext.IsComplexValue)
            {
                NGCSerializerContext context = new NGCSerializerContext(this,
                                                                        serializableObjectContext,
                                                                         SerializerAction.serializePage);

                ((NgcSerializationManagerAsync)SerializationManager).OperationStack.Push(context);
            }
            else
            {
                throw new XpsSerializationException(SR.ReachSerialization_WrongPropertyTypeForPageContent);
            }
        }

        private
        void
        SerializePage(
            SerializableObjectContext   serializableObjectContext
            )
        {
            FixedPage fixedPage = Toolbox.GetPageRoot(serializableObjectContext.TargetObject);

            if(fixedPage != null)
            {
                ReachSerializer serializer = SerializationManager.GetSerializer(fixedPage);

                if(serializer!=null)
                {
                    NgcSerializationManagerAsync manager = SerializationManager as NgcSerializationManagerAsync;
                    
                    XpsSerializationPrintTicketRequiredEventArgs e = 
                        new XpsSerializationPrintTicketRequiredEventArgs(PrintTicketLevel.FixedPagePrintTicket,
                                                         0);
                    manager.OnNGCSerializationPrintTicketRequired(e);

                    Toolbox.Layout(fixedPage, manager.GetActivePrintTicket());

                    serializer.SerializeObject(fixedPage);
                }
                else
                {
                    throw new XpsSerializationException(SR.ReachSerialization_NoSerializer);
                }
            }

        }

        #endregion Internal Methods
    };
}
