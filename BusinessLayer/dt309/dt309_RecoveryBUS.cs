using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt309_RecoveryBUS
    {
        private const string RecoveryOptionNone = "none";
        private const string RecoveryOptionScrap = "scrap";
        private const string RecoveryOptionRestock = "restock";

        private const string RecoveryStatusScheduled = "scheduled";
        private const string RecoveryStatusAwaitManagerConfirm = "await_manager_confirm";
        private const string RecoveryStatusCompleted = "completed";
        private const string RecoveryStatusCancelled = "cancelled";

        private const string SparePartManagerGroupName = "機邊庫【管理】";
        private const string RecoveryTypeUse = "舊品回收";

        private readonly TPLogger logger;
        private static dt309_RecoveryBUS instance;

        public static dt309_RecoveryBUS Instance
        {
            get { return instance ?? (instance = new dt309_RecoveryBUS()); }
            private set { instance = value; }
        }

        private dt309_RecoveryBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        public List<dt309_RecoveryTickets> GetList(
            string deptPrefix,
            string status = "",
            DateTime? plannedDateFrom = null,
            DateTime? plannedDateTo = null,
            string assignedUserId = "")
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var query = context.dt309_RecoveryTickets.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(deptPrefix))
                    {
                        query = query.Where(ticket =>
                            context.dt309_Materials.Any(material =>
                                material.Id == ticket.NewMaterialId &&
                                material.DelTime == null &&
                                material.IdDept != null &&
                                material.IdDept.StartsWith(deptPrefix)));
                    }

                    if (!string.IsNullOrWhiteSpace(status))
                    {
                        query = query.Where(ticket => ticket.Status == status);
                    }

                    if (plannedDateFrom.HasValue)
                    {
                        DateTime dateFrom = plannedDateFrom.Value.Date;
                        query = query.Where(ticket => ticket.PlannedDisposeDate >= dateFrom);
                    }

                    if (plannedDateTo.HasValue)
                    {
                        DateTime dateToExclusive = plannedDateTo.Value.Date.AddDays(1);
                        query = query.Where(ticket => ticket.PlannedDisposeDate < dateToExclusive);
                    }

                    if (!string.IsNullOrWhiteSpace(assignedUserId))
                    {
                        query = query.Where(ticket => ticket.AssignedUserId == assignedUserId);
                    }

                    return query
                        .OrderByDescending(ticket => ticket.CreatedDate)
                        .ThenByDescending(ticket => ticket.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return new List<dt309_RecoveryTickets>();
            }
        }

        public Dictionary<int, int> GetEvidenceCountMap(IEnumerable<int> ticketIds)
        {
            try
            {
                var idList = (ticketIds ?? Enumerable.Empty<int>())
                    .Distinct()
                    .ToList();

                if (idList.Count == 0)
                {
                    return new Dictionary<int, int>();
                }

                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt309_RecoveryEvidence
                        .Where(item => item.IsActive && idList.Contains(item.RecoveryTicketId))
                        .GroupBy(item => item.RecoveryTicketId)
                        .ToDictionary(group => group.Key, group => group.Count());
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return new Dictionary<int, int>();
            }
        }

        public dt309_RecoveryTickets GetTicketById(int id)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt309_RecoveryTickets.FirstOrDefault(ticket => ticket.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return null;
            }
        }

        public List<dt309_RecoveryGuides> GetGuideList(bool activeOnly = true)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var query = context.dt309_RecoveryGuides.AsQueryable();
                    if (activeOnly)
                    {
                        query = query.Where(item => item.IsActive);
                    }

                    return query
                        .OrderBy(item => item.DisplayOrder)
                        .ThenByDescending(item => item.UploadedDate)
                        .ThenByDescending(item => item.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return new List<dt309_RecoveryGuides>();
            }
        }

        public int AddGuide(dt309_RecoveryGuides item)
        {
            if (item == null)
            {
                return -1;
            }

            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    int nextOrder = item.DisplayOrder > 0
                        ? item.DisplayOrder
                        : (context.dt309_RecoveryGuides
                            .Where(guide => guide.IsActive)
                            .Select(guide => (int?)guide.DisplayOrder)
                            .Max() ?? 0) + 1;

                    item.DisplayOrder = nextOrder;
                    item.IsActive = true;
                    if (item.UploadedDate == default(DateTime))
                    {
                        item.UploadedDate = DateTime.Now;
                    }

                    context.dt309_RecoveryGuides.Add(item);
                    context.SaveChanges();
                    return item.Id;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool DeactivateGuide(int id)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var guide = context.dt309_RecoveryGuides.FirstOrDefault(item => item.Id == id);
                    if (guide == null)
                    {
                        return false;
                    }

                    guide.IsActive = false;
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public List<dt309_RecoveryEvidence> GetEvidenceListByTicketId(int ticketId, bool activeOnly = true)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var query = context.dt309_RecoveryEvidence
                        .Where(item => item.RecoveryTicketId == ticketId);

                    if (activeOnly)
                    {
                        query = query.Where(item => item.IsActive);
                    }

                    return query
                        .OrderByDescending(item => item.UploadedDate)
                        .ThenByDescending(item => item.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return new List<dt309_RecoveryEvidence>();
            }
        }

        public bool DeactivateEvidence(int id)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var evidence = context.dt309_RecoveryEvidence.FirstOrDefault(item => item.Id == id);
                    if (evidence == null)
                    {
                        return false;
                    }

                    evidence.IsActive = false;
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool IsManager309(string userId)
        {
            try
            {
                var userGroups = dm_GroupUserBUS.Instance.GetListByUID(userId);
                var managerGroups = dm_GroupBUS.Instance.GetListByName(SparePartManagerGroupName);
                return managerGroups.Any(group => userGroups.Any(userGroup => userGroup.IdGroup == group.Id));
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool CreateIssueTransactionWithRecovery(
            int materialId,
            int sourceStorageId,
            double issueQuantity,
            string desc,
            string userId,
            string recoveryOption,
            string assignedUserId,
            DateTime? plannedDisposeDate,
            int? restockStorageId,
            string recoveryNote,
            out string message)
        {
            message = string.Empty;

            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                using (var tran = context.Database.BeginTransaction())
                {
                    try
                    {
                        var material = context.dt309_Materials.FirstOrDefault(item => item.Id == materialId && item.DelTime == null);
                        if (material == null)
                        {
                            message = "找不到對應物料。";
                            return false;
                        }

                        if (material.IsDisable == true)
                        {
                            message = "停用中的物料不可進行領用。";
                            return false;
                        }

                        if (issueQuantity <= 0)
                        {
                            message = "領用數量需大於零。";
                            return false;
                        }

                        double availableQuantity = sourceStorageId == 1
                            ? material.QuantityInMachine
                            : material.QuantityInStorage;

                        if (issueQuantity > availableQuantity)
                        {
                            message = "領用數量大於庫存數量。";
                            return false;
                        }

                        string normalizedOption = NormalizeRecoveryOption(recoveryOption);
                        ValidateRecoveryRequest(context, material, normalizedOption, assignedUserId, plannedDisposeDate, restockStorageId);

                        var issueTransaction = new dt309_Transactions
                        {
                            MaterialId = material.Id,
                            StorageId = sourceStorageId,
                            TransactionType = "out",
                            Quantity = issueQuantity,
                            CreatedDate = DateTime.Now,
                            UserDo = userId,
                            Desc = NormalizeText(desc)
                        };

                        context.dt309_Transactions.Add(issueTransaction);
                        context.SaveChanges();

                        if (normalizedOption == RecoveryOptionNone)
                        {
                            tran.Commit();
                            return true;
                        }

                        var oldBaseMaterial = material;
                        double oldQuantity = issueQuantity;
                        dt309_Materials recoveryMaterial = null;
                        dt309_Transactions restockTransaction = null;
                        string ticketStatus = RecoveryStatusScheduled;
                        DateTime? actualDisposeDate = null;
                        string resultNote = null;
                        string completedBy = null;
                        DateTime? completedDate = null;

                        if (IsRestock(normalizedOption))
                        {
                            recoveryMaterial = GetOrCreateRecoveryMaterial(context, oldBaseMaterial, userId);
                            context.SaveChanges();

                            restockTransaction = new dt309_Transactions
                            {
                                MaterialId = recoveryMaterial.Id,
                                StorageId = restockStorageId.Value,
                                TransactionType = "in",
                                Quantity = oldQuantity,
                                CreatedDate = DateTime.Now,
                                UserDo = userId,
                                Desc = $"回收入庫 / 原物料:{oldBaseMaterial.Code}"
                            };

                            context.dt309_Transactions.Add(restockTransaction);
                            context.SaveChanges();

                            ticketStatus = RecoveryStatusCompleted;
                            actualDisposeDate = DateTime.Now;
                            resultNote = "回收入庫已完成。";
                            completedBy = userId;
                            completedDate = DateTime.Now;
                        }

                        string ticketNo = BuildTicketNo();
                        var ticket = new dt309_RecoveryTickets
                        {
                            TicketNo = ticketNo,
                            IssueTransactionId = issueTransaction.Id,
                            RestockInTransactionId = restockTransaction?.Id,
                            NewMaterialId = material.Id,
                            OldBaseMaterialId = oldBaseMaterial.Id,
                            OldRecoveryMaterialId = recoveryMaterial?.Id,
                            RecoveryOption = normalizedOption,
                            Quantity = oldQuantity,
                            SourceStorageId = sourceStorageId,
                            RestockStorageId = restockStorageId,
                            AssignedUserId = NormalizeText(assignedUserId),
                            PlannedDisposeDate = plannedDisposeDate,
                            ActualDisposeDate = actualDisposeDate,
                            Status = ticketStatus,
                            Description = BuildTicketDescription(desc, recoveryNote),
                            ResultNote = resultNote,
                            CompletedBy = completedBy,
                            CompletedDate = completedDate,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = userId,
                            UpdatedDate = DateTime.Now
                        };

                        context.dt309_RecoveryTickets.Add(ticket);
                        issueTransaction.Desc = AppendTicketReference(issueTransaction.Desc, ticketNo);
                        context.SaveChanges();

                        tran.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        logger.Error(nameof(CreateIssueTransactionWithRecovery), ex.ToString());
                        message = ex.Message;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(nameof(CreateIssueTransactionWithRecovery), ex.ToString());
                message = "建立領用與回收單失敗。";
                return false;
            }
        }

        public bool UploadEvidence(
            int ticketId,
            DateTime actualDisposeDate,
            string resultNote,
            string uploadedBy,
            List<dt309_RecoveryEvidence> evidenceItems,
            out string message)
        {
            message = string.Empty;

            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                using (var tran = context.Database.BeginTransaction())
                {
                    try
                    {
                        var ticket = context.dt309_RecoveryTickets.FirstOrDefault(item => item.Id == ticketId);
                        if (ticket == null)
                        {
                            message = "找不到對應回收單。";
                            return false;
                        }

                        if (!IsScrap(ticket.RecoveryOption))
                        {
                            message = "只有報廢案件可上傳證明。";
                            return false;
                        }

                        if (string.Equals(ticket.Status, RecoveryStatusCancelled, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(ticket.Status, RecoveryStatusCompleted, StringComparison.OrdinalIgnoreCase))
                        {
                            message = "目前狀態不可上傳證明。";
                            return false;
                        }

                        if (actualDisposeDate == default(DateTime))
                        {
                            message = "請填寫實際報廢時間。";
                            return false;
                        }

                        var validEvidenceItems = (evidenceItems ?? new List<dt309_RecoveryEvidence>())
                            .Where(item => item != null)
                            .ToList();

                        if (validEvidenceItems.Count == 0)
                        {
                            message = "至少需要一份證明文件。";
                            return false;
                        }

                        foreach (var item in validEvidenceItems)
                        {
                            context.dt309_RecoveryEvidence.Add(new dt309_RecoveryEvidence
                            {
                                RecoveryTicketId = ticketId,
                                ActualName = item.ActualName ?? string.Empty,
                                EncryptionName = item.EncryptionName ?? string.Empty,
                                FileExt = item.FileExt ?? string.Empty,
                                UploadedBy = string.IsNullOrWhiteSpace(item.UploadedBy) ? uploadedBy : item.UploadedBy,
                                UploadedDate = item.UploadedDate == default(DateTime) ? DateTime.Now : item.UploadedDate,
                                IsActive = true
                            });
                        }

                        ticket.ActualDisposeDate = actualDisposeDate;
                        ticket.ResultNote = NormalizeText(resultNote);
                        ticket.EvidenceSubmittedDate = DateTime.Now;
                        ticket.Status = RecoveryStatusAwaitManagerConfirm;
                        ticket.UpdatedBy = uploadedBy;
                        ticket.UpdatedDate = DateTime.Now;

                        context.SaveChanges();
                        tran.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        logger.Error(nameof(UploadEvidence), ex.ToString());
                        message = ex.Message;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(nameof(UploadEvidence), ex.ToString());
                message = "上傳證明失敗。";
                return false;
            }
        }

        public bool UpdateSchedule(int ticketId, string assignedUserId, DateTime plannedDisposeDate, string updatedBy, out string message)
        {
            message = string.Empty;

            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var ticket = context.dt309_RecoveryTickets.FirstOrDefault(item => item.Id == ticketId);
                    if (ticket == null)
                    {
                        message = "找不到對應回收單。";
                        return false;
                    }

                    if (!IsScrap(ticket.RecoveryOption) ||
                        !string.Equals(ticket.Status, RecoveryStatusScheduled, StringComparison.OrdinalIgnoreCase))
                    {
                        message = "只有已安排的報廢案件可更新日期。";
                        return false;
                    }

                    ValidateAssignedUser(context, assignedUserId);

                    ticket.AssignedUserId = NormalizeText(assignedUserId);
                    ticket.PlannedDisposeDate = plannedDisposeDate;
                    ticket.UpdatedBy = updatedBy;
                    ticket.UpdatedDate = DateTime.Now;
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(nameof(UpdateSchedule), ex.ToString());
                message = ex.Message;
                return false;
            }
        }

        public bool ConfirmCompleted(int ticketId, string currentUserId, string resultNote, out string message)
        {
            message = string.Empty;

            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var ticket = context.dt309_RecoveryTickets.FirstOrDefault(item => item.Id == ticketId);
                    if (ticket == null)
                    {
                        message = "找不到對應回收單。";
                        return false;
                    }

                    if (!IsScrap(ticket.RecoveryOption))
                    {
                        message = "只有報廢案件需要管理確認。";
                        return false;
                    }

                    if (!string.Equals(ticket.Status, RecoveryStatusAwaitManagerConfirm, StringComparison.OrdinalIgnoreCase))
                    {
                        message = "目前狀態不可確認完成。";
                        return false;
                    }

                    bool isManager = IsManager309(currentUserId);
                    bool isCreator = string.Equals(ticket.CreatedBy, currentUserId, StringComparison.OrdinalIgnoreCase);
                    if (!isManager && !isCreator)
                    {
                        message = "只有管理者或建立人可確認完成。";
                        return false;
                    }

                    int evidenceCount = context.dt309_RecoveryEvidence.Count(item =>
                        item.RecoveryTicketId == ticketId && item.IsActive);

                    if (evidenceCount <= 0)
                    {
                        message = "至少需要一份證明文件才可確認完成。";
                        return false;
                    }

                    ticket.Status = RecoveryStatusCompleted;
                    if (!string.IsNullOrWhiteSpace(resultNote))
                    {
                        ticket.ResultNote = NormalizeText(resultNote);
                    }

                    ticket.CompletedBy = currentUserId;
                    ticket.CompletedDate = DateTime.Now;
                    ticket.UpdatedBy = currentUserId;
                    ticket.UpdatedDate = DateTime.Now;
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(nameof(ConfirmCompleted), ex.ToString());
                message = ex.Message;
                return false;
            }
        }

        public bool CancelTicket(int ticketId, string currentUserId, string cancelReason, out string message)
        {
            message = string.Empty;

            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var ticket = context.dt309_RecoveryTickets.FirstOrDefault(item => item.Id == ticketId);
                    if (ticket == null)
                    {
                        message = "找不到對應回收單。";
                        return false;
                    }

                    if (string.Equals(ticket.Status, RecoveryStatusCompleted, StringComparison.OrdinalIgnoreCase))
                    {
                        message = "已完成案件不可取消。";
                        return false;
                    }

                    if (IsRestock(ticket.RecoveryOption))
                    {
                        message = "回收入庫案件已直接入庫，不可取消。";
                        return false;
                    }

                    ticket.Status = RecoveryStatusCancelled;
                    ticket.CancelledBy = currentUserId;
                    ticket.CancelledDate = DateTime.Now;
                    ticket.CancelReason = NormalizeText(cancelReason);
                    ticket.UpdatedBy = currentUserId;
                    ticket.UpdatedDate = DateTime.Now;
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(nameof(CancelTicket), ex.ToString());
                message = ex.Message;
                return false;
            }
        }

        private string AppendTicketReference(string desc, string ticketNo)
        {
            string reference = $"回收單號: {ticketNo}";
            if (string.IsNullOrWhiteSpace(desc))
            {
                return reference;
            }

            if (desc.Contains(reference))
            {
                return desc;
            }

            return $"{desc} / {reference}";
        }

        private string BuildTicketDescription(string issueDesc, string recoveryNote)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(issueDesc))
            {
                parts.Add(issueDesc.Trim());
            }

            if (!string.IsNullOrWhiteSpace(recoveryNote))
            {
                parts.Add($"回收備註: {recoveryNote.Trim()}");
            }

            return string.Join(" / ", parts.Where(item => !string.IsNullOrWhiteSpace(item)));
        }

        private string BuildTicketNo()
        {
            return $"RCV-{DateTime.Now:yyyyMMddHHmmssfff}";
        }

        private string NormalizeRecoveryOption(string recoveryOption)
        {
            string value = NormalizeText(recoveryOption).ToLowerInvariant();
            if (value == RecoveryOptionScrap)
            {
                return RecoveryOptionScrap;
            }

            if (value == RecoveryOptionRestock)
            {
                return RecoveryOptionRestock;
            }

            return RecoveryOptionNone;
        }

        private string NormalizeText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private void ValidateRecoveryRequest(
            DBDocumentManagementSystemEntities context,
            dt309_Materials material,
            string recoveryOption,
            string assignedUserId,
            DateTime? plannedDisposeDate,
            int? restockStorageId)
        {
            if (recoveryOption == RecoveryOptionNone)
            {
                return;
            }

            if (material == null)
            {
                throw new Exception("找不到對應物料。");
            }

            if (material.IsDisable == true)
            {
                throw new Exception("停用中的物料不可建立回收單。");
            }

            if (IsScrap(recoveryOption))
            {
                ValidateAssignedUser(context, assignedUserId);

                if (!plannedDisposeDate.HasValue)
                {
                    throw new Exception("請填寫預計報廢日期。");
                }
            }
            else if (IsRestock(recoveryOption))
            {
                if (!restockStorageId.HasValue)
                {
                    throw new Exception("請選擇回收入庫倉庫。");
                }

                bool storageExists = context.dt309_Storages.Any(item => item.Id == restockStorageId.Value);
                if (!storageExists)
                {
                    throw new Exception("找不到回收入庫倉庫。");
                }
            }
            else
            {
                throw new Exception("不支援的回收處理方式。");
            }
        }

        private void ValidateAssignedUser(DBDocumentManagementSystemEntities context, string assignedUserId)
        {
            if (string.IsNullOrWhiteSpace(assignedUserId))
            {
                throw new Exception("請選擇經辦人。");
            }

            bool userExists = context.dm_User.Any(item => item.Id == assignedUserId && (item.Status == null || item.Status == 0));
            if (!userExists)
            {
                throw new Exception("找不到對應經辦人。");
            }
        }

        private dt309_Materials GetOrCreateRecoveryMaterial(DBDocumentManagementSystemEntities context, dt309_Materials oldBaseMaterial, string updatedBy)
        {
            string baseCode = NormalizeText(oldBaseMaterial.Code);
            string expectedCode = string.IsNullOrWhiteSpace(baseCode)
                ? $"OLD-{oldBaseMaterial.Id}"
                : $"{baseCode}-OLD";

            var recoveryMaterial = context.dt309_Materials.FirstOrDefault(item =>
                item.DelTime == null &&
                ((item.BaseMaterialId == oldBaseMaterial.Id && item.IsRecoveredOld) ||
                 (item.IdDept == oldBaseMaterial.IdDept && item.Code == expectedCode)));

            if (recoveryMaterial != null)
            {
                recoveryMaterial.BaseMaterialId = oldBaseMaterial.Id;
                recoveryMaterial.IsRecoveredOld = true;
                recoveryMaterial.IsDisable = false;
                recoveryMaterial.EnabledBy = updatedBy;
                recoveryMaterial.EnabledDate = DateTime.Now;
                return recoveryMaterial;
            }

            recoveryMaterial = new dt309_Materials
            {
                IdDept = NormalizeText(oldBaseMaterial.IdDept),
                Code = BuildRecoveryMaterialCode(context, oldBaseMaterial),
                DisplayName = $"{NormalizeText(oldBaseMaterial.DisplayName)} (舊品)",
                TypeUse = RecoveryTypeUse,
                Location = NormalizeText(oldBaseMaterial.Location),
                IdUnit = oldBaseMaterial.IdUnit,
                MinQuantity = 0,
                QuantityInStorage = 0,
                QuantityInMachine = 0,
                ExpDate = null,
                IdManager = NormalizeText(oldBaseMaterial.IdManager),
                DelTime = null,
                UserDel = null,
                Price = 0,
                IsDisable = false,
                DisabledBy = null,
                DisabledDate = null,
                EnabledBy = updatedBy,
                EnabledDate = DateTime.Now,
                ReplacementMaterialId = null,
                ReplacementDate = null,
                BaseMaterialId = oldBaseMaterial.Id,
                IsRecoveredOld = true
            };

            context.dt309_Materials.Add(recoveryMaterial);
            return recoveryMaterial;
        }

        private string BuildRecoveryMaterialCode(DBDocumentManagementSystemEntities context, dt309_Materials oldBaseMaterial)
        {
            string baseCode = NormalizeText(oldBaseMaterial.Code);
            string seed = string.IsNullOrWhiteSpace(baseCode)
                ? $"OLD-{oldBaseMaterial.Id}"
                : $"{baseCode}-OLD";
            string code = seed;
            int suffix = 2;

            while (context.dt309_Materials.Any(item =>
                item.DelTime == null &&
                item.IdDept == oldBaseMaterial.IdDept &&
                item.Code == code))
            {
                code = $"{seed}-{suffix++}";
            }

            return code;
        }

        private bool IsScrap(string recoveryOption)
        {
            return string.Equals(recoveryOption, RecoveryOptionScrap, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsRestock(string recoveryOption)
        {
            return string.Equals(recoveryOption, RecoveryOptionRestock, StringComparison.OrdinalIgnoreCase);
        }
    }
}
