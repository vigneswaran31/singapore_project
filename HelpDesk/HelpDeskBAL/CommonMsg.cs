using HelpDeskEntity.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskBAL
{
    public class CommonMsg
    {
        public static string Authorize_Deletion_MSG(string moduleName)
        {
            return "You are not authorized to delete this record. Only the person who has created this record or the site administrator can do it.";
        }

        public static string DependancyError()
        {
            return "The record can not be deleted, it has related data, please delet the related records first.";
        }

        public static string Error(string a = null)
        {
            return "Some error occurred while processing your request.Please try again.";
        }

        public static string Success(string entity, En_CRUD? CRUDType = null)
        {
            if (CRUDType == En_CRUD.Insert)
                return Success_Insert(entity);
            else if (CRUDType == En_CRUD.Update)
                return Success_Update(entity);
            else if (CRUDType == En_CRUD.Delete)
                return Success_Delete(entity);
            else
                return "Your Request has been successfully processed.";
        }

        public static string Success_Insert(string entity)
        {
            return entity + " " + "has been inserted successfully.";
        }

        public static string Success_Update(string entity)
        {
            return entity + " " + "has been updated successfully.";
        }

        public static string Success_Delete(string entity)
        {
            return entity + " " + "has been deleted successfully.";
        }

        public static string Fail(string entity, En_CRUD CRUDType)
        {
            if (CRUDType == En_CRUD.Insert)
                return Fail_Insert(entity);
            else if (CRUDType == En_CRUD.Update)
                return Fail_Update(entity);
            else if (CRUDType == En_CRUD.Delete)
                return Fail_Delete(entity);
            else
                return "Your Request has been Successfully processed.";
        }

        public static string Fail_Insert(string entity)
        {
            return "Some error occured while inserting " + entity + ". Please try again.";
        }

        public static string Fail_Update(string entity)
        {
            return "Some error occured while updating " + entity + ". Please try again.";
        }

        public static string Fail_Delete(string entity)
        {
            return "Some error occured while deleting " + entity + ". Please try again.";
        }
    }
}
