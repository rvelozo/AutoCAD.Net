        public static ObjectId ExtrudeRegionByHeight(double extrusionDistance, ObjectId objId, Database acCurDb, string layer, bool isBlock)
        {
            ObjectId blkRecId = ObjectId.Null;
            DBObjectCollection dbObjCol = new DBObjectCollection();

            ObjectId regionId = new ObjectId();
            if (isBlock)
            {
                regionId = Blocks.GetRegionFromBlockReference(objId, acCurDb);
            }
            else
            {
                regionId = objId;
            }

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
                        if (acObjId != regionId) continue;

                        Solid3d solid = new Solid3d() { Layer = layer };
                        Region region = (Region)acTrans.GetObject(acObjId, OpenMode.ForWrite);

                        solid.Extrude(region, extrusionDistance, 0);
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

                //ed.WriteMessage("Exception on BLOCKS  ");
            }
            return blkRecId;
        }
