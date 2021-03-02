      public static ObjectId InsertExternalBlock(Database acCurDb, string blockPath, string blockName, Point3d insPt, Scale3d scale3D, Matrix3d matrix3D, string layer, bool insertOnly)
        {
            if (string.IsNullOrWhiteSpace(blockPath)) return ObjectId.Null;

            ObjectId blkRecId = ObjectId.Null;

            ObjectId acXrefId = acCurDb.AttachXref(blockPath, blockName);
            BlockReference acBlkRef = new BlockReference(insPt, acXrefId) { Layer = layer, ScaleFactors = scale3D };
            if (acXrefId.IsNull) return blkRecId;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                using (acBlkRef)
                {
                    BlockTableRecord acBlkTblRec = GetAcBlockTableRecord(acCurDb, acTrans, OpenMode.ForRead, ModelSpace: true);

                    acBlkRef.TransformBy(matrix3D);
                    acBlkTblRec.AppendEntity(acBlkRef);

                    acTrans.AddNewlyCreatedDBObject(acBlkRef, true);

                    if (!insertOnly)
                    {
                        blkRecId = acBlkRef.Id;
                    }
                    else
                    {
                        DBObjectCollection dbObjCol = new DBObjectCollection();
                        Entity ent = (Entity)acTrans.GetObject(acBlkRef.Id, OpenMode.ForWrite);
                        ent.Explode(dbObjCol);
                        string l = ent.Layer;
                        ent.Erase(true);
                        foreach (DBObject dbObj in dbObjCol)
                        {
                            Entity acEnt = dbObj as Entity;
                            acBlkTblRec.AppendEntity(acEnt);
                            acTrans.AddNewlyCreatedDBObject(dbObj, true);
                            acEnt = acTrans.GetObject(dbObj.ObjectId, OpenMode.ForWrite) as Entity;
                            acEnt.Layer = l;
                            blkRecId = acEnt.Id;
                        }
                    }
                }

                using (ObjectIdCollection acXrefIdCol = new ObjectIdCollection())
                {
                    acXrefIdCol.Add(acXrefId);
                    acCurDb.BindXrefs(acXrefIdCol, false);
                }

                acTrans.Commit();
            }

            return blkRecId;
        }
