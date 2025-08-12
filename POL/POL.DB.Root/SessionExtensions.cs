using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Generators;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;

namespace POL.DB.Root
{
    public static class SessionExtensions
    {
        public static int GetObjectCount<T>(this Session session, CriteriaOperator criteria = null)
        {
            return (int) session.Evaluate<T>(CriteriaOperator.Parse("Count()"), criteria);
        }

        public static PropertyValueStore CreatePropertyValueStore(XPClassInfo classInfo,
            MemberInitExpression memberInitExpression)
        {
            var propertyValueStore = new PropertyValueStore();

            foreach (var binding in memberInitExpression.Bindings)
            {
                var assignment = binding as MemberAssignment;
                if (binding == null)
                {
                    throw new Exception(
                        "All bindings inside the MemberInitExpression are expected to be of type MemberAssignment.");
                }

                var memberName = binding.Member.Name;
                var memberInfo = classInfo.GetMember(memberName);
                if (memberInfo == null)
                    throw new ArgumentOutOfRangeException(memberName,
                        string.Format("The member {0} of the {1} class could not be found.", memberName,
                            classInfo.FullName));

                if (!memberInfo.IsPersistent)
                    throw new ArgumentException(memberName,
                        string.Format("The member {0} of the {1} class is not persistent.", memberName,
                            classInfo.FullName));

                var constant = Expression.Lambda(assignment.Expression, null).Compile().DynamicInvoke();

                propertyValueStore.Add(new KeyValuePair<XPMemberInfo, object>(memberInfo, constant));
            }
            return propertyValueStore;
        }

        public static ModificationResult Delete<T>(this Session session, CriteriaOperator criteria) where T : IXPObject
        {
            if (ReferenceEquals(criteria, null))
                criteria = CriteriaOperator.Parse("True");
            var classInfo = session.GetClassInfo(typeof (T));
            var batchWideData = new BatchWideDataHolder4Modification(session);
            var recordsAffected = (int) session.Evaluate<T>(CriteriaOperator.Parse("Count()"), criteria);

            var collection = DeleteQueryGenerator.GenerateDelete(classInfo,
                ObjectGeneratorCriteriaSet.GetCommonCriteriaSet(criteria), batchWideData);

            foreach (var item in collection)
            {
                item.RecordsAffected = recordsAffected;
            }
            var collectionToArray = collection.ToArray<ModificationStatement>();
            var result = session.DataLayer.ModifyData(collectionToArray);
            return result;
        }

        public static ModificationResult Update<T>(this Session session, Expression<Func<T>> evaluator,
            CriteriaOperator criteria) where T : IXPObject
        {
            if (ReferenceEquals(criteria, null))
                criteria = CriteriaOperator.Parse("True");

            var classInfo = session.GetClassInfo(typeof (T));
            var batchWideData = new BatchWideDataHolder4Modification(session);
            var recordsAffected = (int) session.Evaluate<T>(CriteriaOperator.Parse("Count()"), criteria);

            PropertyValueStore propertyValueStore = null;
            var memberInitCount = 1;
            evaluator.Visit<MemberInitExpression>(expression =>
            {
                if (memberInitCount > 1)
                {
                    throw new Exception("Only a single MemberInitExpression is allowed for the evaluator parameter.");
                }
                memberInitCount++;
                propertyValueStore = CreatePropertyValueStore(classInfo, expression);
                return expression;
            });

            var properties = new MemberInfoCollection(classInfo, propertyValueStore.Select(x => x.Key).ToArray());

            var collection = UpdateQueryGenerator.GenerateUpdate(classInfo, properties,
                ObjectGeneratorCriteriaSet.GetCommonCriteriaSet(criteria), batchWideData);

            foreach (var updateStatement in collection.OfType<UpdateStatement>())
            {
                for (var i = 0; i < updateStatement.Parameters.Count; i++)
                {
                    var value = propertyValueStore[i].Value;
                    if (value is IXPObject)
                        updateStatement.Parameters[i].Value = ((IXPObject) value).ClassInfo.GetId(value);
                    else
                        updateStatement.Parameters[i].Value = value;
                }
                updateStatement.RecordsAffected = recordsAffected;
            }
            return session.DataLayer.ModifyData(collection.ToArray<ModificationStatement>());
        }
    }
}
