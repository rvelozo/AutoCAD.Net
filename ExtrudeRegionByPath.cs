        public static ObjectId ExtrudeRegionByPath(Database acCurDb, ObjectId regionBlockId, ObjectId path, string layer)
        {
            ObjectId blkRecId = ObjectId.Null;
            DBObjectCollection dbObjCol = new DBObjectCollection();

            try
            {
                Transaction acTrans = acCurDb.TransactionManager.StartTransaction();
                using (acTrans)
                {
                    BlockTableRecord acBlkTblRec = Blocks.GetAcBlockTableRecord(acCurDb, acTrans, OpenMode.ForRead, ModelSpace: true);

                    BlockTable bt = (BlockTable)acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord ms = (BlockTableRecord)acTrans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                    foreach (ObjectId acObjId in acBlkTblRec)
                    {
                        if (acObjId != regionBlockId) continue;

                        Solid3d solid = new Solid3d() { Layer = layer };
                        Region region = (Region)acTrans.GetObject(acObjId, OpenMode.ForWrite);
                        Curve pathEnt = (Curve)acTrans.GetObject(path, OpenMode.ForWrite);
                        
                        solid.ExtrudeAlongPath(region, pathEnt, 0);

                        region.Erase(true);
                        ms.AppendEntity(solid);
                        acTrans.AddNewlyCreatedDBObject(solid, true);
                        blkRecId = solid.Id;

                        break;
                    }

                    acTrans.Commit();
                }
            }
            catch (Exception ex)
            {
                return new ObjectId();
            }
            return blkRecId;
        }
