using HelpDeskEntity;
using HelpDeskEntity.Classes;
using HelpDeskEntity.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskBAL
{
    public class CommonBL
    {
        //Check New Record or Existing based on entityId
        public bool isNewEntry(int entityId)
        {
            if (entityId<=0)
                return true;
            else
                return false;
        }

        //Check if given path directories exists, if not then create it
        public void EnsureDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete all files under directory
        public void EnsureEmptyDirectory(string path)
        {
            try
            {
                Array.ForEach(Directory.GetFiles(path), System.IO.File.Delete);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete Directory and all sub directory
        public void DeleteDirectory(string path,bool subDirectories = false)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, subDirectories);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // If File with same name already exists then appends #no to file name
        public string GetUniqueDocumentName(string fileName,string dirPath)
        {
            try
            {
                bool isFileExists = true;
                int i=0;
                string filepath = Path.Combine(dirPath, fileName);
                while (isFileExists)
                {
                    if (File.Exists(filepath))
                    {
                        i += 1;
                        filepath = Path.Combine(dirPath, Path.GetFileNameWithoutExtension(fileName)) + "_" + i.ToString() + Path.GetExtension(fileName);
                    }
                    else
                        isFileExists = false;
                    
                }

                return filepath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Check to if Permission is allow for Operator by OperatorId.
        public bool IsPermissionAllow(int OperatorId, int PermissionType)
        {
            OptPermission oOptPermission = new OptPermission();
            oOptPermission.PermissionId = new OperatorPermissionBL().GetOperatorPermissionsByOptId(OperatorId);
            if (oOptPermission.PermissionId.Contains(PermissionType))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
