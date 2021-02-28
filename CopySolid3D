  public static List<ObjectId> CopySolid3D(Database acCurDb, ObjectId solidId, Vector3dCollection toPoints)
        {
            List<ObjectId> copiedIds = new List<ObjectId>();
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTableRecord acBlkTblRec = Blocks.GetAcBlockTableRecord(acCurDb, acTrans, OpenMode.ForWrite, true);

                Solid3d solidBase = (Solid3d)acTrans.GetObject(solidId, OpenMode.ForWrite);

                foreach (Vector3d v in toPoints)
                {
                    Solid3d solidCopy = solidBase.Clone() as Solid3d;
                    solidCopy.TransformBy(Matrix3d.Displacement(v));

                    acBlkTblRec.AppendEntity(solidCopy);
                    acTrans.AddNewlyCreatedDBObject(solidCopy, true);

                    copiedIds.Add(solidCopy.Id);
                }
                acTrans.Commit();
            }

            return copiedIds;
        }
