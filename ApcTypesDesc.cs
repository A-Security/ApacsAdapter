﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacsAdapter
{
    partial class ApcGetData
    {
        private string getTypeDesc(string strType)
        {
            return apcTypeDescDict.TryGetValue(strType, out strType) ? strType : null;
        }
        public Dictionary<string, string> apcTypeDescDict = new Dictionary<string, string>()
            {
                {"TApcBolidEvent_Entry", "Проход"},
                {"TApcCardHolderAccess_DeniedNonEscortedVisitWhileVisit_ErrHolder", "Доступ запрещен, предъявлена карта посетителя, которому не нужен сопровождающий, владелец карты не найден"},
                {"TApcEvent_Del", "Системный контейнер"},
                {"TApcVISDrv_OffLine", "Нет связи с сервером NVR"},
                {"TApcBolidEvent_AccessGranted", "Доступ предоставлен"},
                {"TApcCardHolderAccess_GrantedUnderDuressNoEntry", "Доступ разрешён, вход под принуждением, проход не осуществлён"},
                {"TApcCardHolderAccess_TimedAPBViol_ErrHolder", "Нарушение временного КПВ, владелец карты не найден"},
                {"TApcVEvAccess_DenyAccUnknownReader", "Доступ запрещён, неизвестный считыватель"},
                {"TAplMCEventStatusChangeLog_CommOnLine", "Связь восстановлена"},
                {"TApcBolidEvent_InputAlarm", "Тревога входа"},
                {"TApcCardHolderAccess_DeniedNotInFile", "Доступ запрещён, карта неизвестна контроллеру"},
                {"TApcEvAccordInput_Alarm", "Тревога на входе"},
                {"TApcISSCam_Disarmed", "Камера ISS снята с охраны"},
                {"TApcISSSocketDrv_OffLine", "Нет связи с сервером ISS"},
                {"TApcVISCam_StartMotion", "Обнаружено движение на камере NVR"},
                {"TApcBolidEvent_FallTemperature", "Понижение температуры"},
                {"TApcCardHolderAccess_DeniedVisitWrongVisitGroup", "Доступ посетителю запрещён, ошибка группы посетителей"},
                {"TApcCardHolderAccess_GrantedVisitAcc", "Доступ посетителю разрешён"},
                {"TAplMCEventAccReqFCLog_DeniedWrongFC", "Доступ запрещён, неверный код организации"},
                {"TApcCardHolderAccess_GrantedEscortAccDuressNoEntry", "Доступ сопровождающему разрешён, вход под принуждением, проход не осуществлён"},
                {"TApcITVCam_LinkRec", "Видеофрагмент с камеры ITV связан с сообщением"},
                {"TAplMCEventLEDUnack", "Не подтверждена лампочка статусной панели ASA-72"},
                {"TApcBolidEvent_SubstDevice", "Подмена прибора"},
                {"TApcCardHolderAccess_GrantedVisitAccDuress_ErrHolder", "Доступ посетителю разрешён, вход под принуждением, владелец карты не найден"},
                {"TApcCardHolderAccess_GrantedVisitAccNoEntry_ErrHolder", "Доступ посетителю разрешён, проход не осуществлён, владелец карты не найден"},
                {"TApcBolidEvent_FaultPowerSupply", "Неисправность источника питания"},
                {"TApcBolidEvent_SensorsFault", "Неисправность термометра"},
                {"TApcCardHolderAccess_Granted_ErrHolder", "Доступ разрешён, владелец карты не найден"},
                {"TApcEventLogin_Err", "Ошибка авторизации оператора"},
                {"TApcVEvTask_Stop", "Служба остановлена"},
                {"TAplMCEventStatusChangeLog_FaultOpenLine", "Обрыв на входе"},
                {"TApcBolidEvent_SecureFOCtrlZone", "Восстановление зоны контроля взлома"},
                {"TAplMCEventStatusChangeLog_FaultShortedLine", "Вход закорочен"},
                {"TApcVISDrv_OnLine", "Есть связь с сервером NVR"},
                {"TAplMCEventExecIV", "ВП выполнена по команде компьютера или зоны КПВ"},
                {"TApcBolidEvent_DoorBlocked", "Дверь заблокирована"},
                {"TApcBolidEvent_StartResetASPT", "Сброс пуска АСПТ"},
                {"TApcBolidEvent_TamperAlarm", "Тревога взлома корпуса"},
                {"TApcTACardHolderRef", "Сообщение УРВ (информация о владельце)"},
                {"TAplMCEventAccReqNoCardLog_EPBReqDoorUsed", "Доступ разрешен по кнопке выхода, проход осуществлён"},
                {"TApcCardHolderAccess_DeniedAreaLimit", "Доступ запрещён, достигнуто ограничение зоны доступа КПВ"},
                {"TApcBolidEventXMLRPCServerStatus_OnLine", "Есть связь с Сервером Орион"},
                {"TApcCardHolderAccess_GrantedEscortAccAPBViolDoorNotUsed_ErrHolder", "Доступ сопровождающему разрешён, ошибка КПВ, проход не осуществлён, владелец карты не найден"},
                {"TApcCardHolderAccess_GrantedEscortAccDuress", "Доступ сопровождающему разрешён, вход под принуждением"},
                {"TApcVEvTask_Restart", "Служба перезапущена"},
                {"TApcBolidEvent_FireOff", "Тушение"},
                {"TApcCardHolderHostAccReq_DeniedWrongTID", "Ошибка доступа, неверный идентификатор транзакции"},
                {"TApcVEvAccess_RealAPBViolExit", "Нарушение зонального КПВ, выход"},
                {"TApcBolidEvent_Undefined", "Неопределенное"},
                {"TApcCardHolderAccess_GrantedVisitAccDuressNoEntry_ErrHolder", "Доступ посетителю разрешён, вход под принуждением, проход не осуществлён, владелец карты не найден"},
                {"TApcEvAccordExtBlock_Norm", "Блок-расширитель в норме"},
                {"TApcMirasysSocketDrv_OffLine", "Нет связи с сервером Mirasys"},
                {"TAplSCEvStatusChange_NotConf", "Вход не конфигурирован"},
                {"TApcBolidEvent_BlockStart", "Блокировка пуска"},
                {"TApcCardHolderAccess_DeniedAreaLimit_ErrHolder", "Доступ запрещён, достигнуто ограничение зоны доступа КПВ, владелец карты не найден"},
                {"TApcISSCam_StartRec", "Начата запись видео на камере ISS"},
                {"TApcVISCam_LinkRec", "Видеофрагмент с камеры NVR связан с сообщением"},
                {"TApcCardHolderAccess_DeniedEscortOrNormCHWhileVisit_ErrHolder", "Доступ запрещен, предъявлена обычная карта или неправильная карта сопровождающего, владелец карты не найден"},
                {"TAplMCEventContrStatus_OnLine", "Есть связь с основным контроллером"},
                {"TApcBolidEvent_AlarmRCS", "Срабатывание СДУ"},
                {"TApcCardHolderHostAccReq_Denied", "Доступ запрещен с компьютера"},
                {"TAplMCEventReaderModeChange", "Контроллером изменён режим считывателя"},
                {"TAplSCEvAccReqNoCard_Diddle", "Попытка подбора кода"},
                {"TApcAuditEvent_Execute", "Аудит выполнения команды на объекте"},
                {"TApcCardHolderAccess_DeniedAPB", "Доступ запрещён, ошибка КПВ"},
                {"TApcCardHolderAccess_GrantedEscortAccNoEntry_ErrHolder", "Доступ сопровождающему разрешён, проход не осуществлён, владелец карты не найден"},
                {"TApcCardHolderAccess_WillGranted", "Будет доступ"},
                {"TAplMCEventStatusChangeLog_FaultNonSpecific", "Ошибка на входе"},
                {"TApcBolidEvent_Armed", "Взят"},
                {"TApcCardHolderAccess_DeniedAPB_ErrHolder", "Доступ запрещён, ошибка КПВ, владелец карты не найден"},
                {"TApcEvAccordExtBlock_Online", "Блок-расширитель на связи"},
                {"TApcVEvInput_Deactiv", "Вход деактивирован"},
                {"TAplMCEventFloorUsed", "Была нажата кнопка выбора этажа"},
                {"TApcBolidEvent_AttentionFire", "Внимание, пожар"},
                {"TApcCardHolderAccess_DeniedVisitNoEscort", "Доступ посетителю запрещён, нет сопровождающего"},
                {"TApcCardHolderAccess_GrantedVisitAccNoEntry", "Доступ посетителю разрешён, проход не осуществлён"},
                {"TApcBolidEvent_NormTemperature", "Температура в норме"},
                {"TApcBolidEvent_RecoverInsideZone", "Восстановление внутренней зоны"},
                {"TApcBolidEvent_RiseTemperature", "Повышение температуры"},
                {"TApcCardHolderAccess_GrantedAPBViolDoorUsed", "Доступ разрешён, ошибка КПВ, проход осуществлён"},
                {"TApcCardHolderAccess_GrantedAPBViolDoorUsed_ErrHolder", "Доступ разрешён, ошибка КПВ, проход осуществлён, владелец карты не найден"},
                {"TApcCardHolderAccess_GrantedVisitAccDuressNoEntry", "Доступ посетителю разрешён, вход под принуждением, проход не осуществлён"},
                {"TApcITVCamZone_Disarmed", "Зона детектора ITV снята с охраны"},
                {"TAplMCEventCardsDownload_Started", "Загрузка карт начата"},
                {"TApcBolidEvent_AccessClosed", "Доступ закрыт"},
                {"TApcBolidEvent_FireEquipFault", "Пожарное  оборудование неисправно"},
                {"TApcBolidEvent_TriggerOn", "Сработал датчик"},
                {"TApcCardHolderAccess_DeniedEscortOrNormCHWhileVisit", "Доступ запрещен, предъявлена обычная карта или неправильная карта сопровождающего"},
                {"TApcCardHolderAccess_DeniedExp_ActDate", "Доступ запрещён, неправильный срок действия карты"},
                {"TApcCardHolderAccess_DeniedReaderExclusList", "Доступ запрещён, карта в списке исключений"},
                {"TApcCardHolderAccess_GrantedVisitAccAPBViolDoorUsed", "Доступ посетителю разрешён, ошибка КПВ, проход осуществлён"},
                {"TApcCardHolderAccess_HostAccReq", "Запрос доступа на компьютер"},
                {"TApcEvAccordExtBlock_Offline", "Блок-расширитель не на связи"},
                {"TApcEvAccordInput_MaskWarning", "Внимание, вход снят с охраны"},
                {"TApcITVCamZone_Alarm", "Тревога на зоне детектора ITV"},
                {"TAplMCEventLogExecIV", "Лог запуска внутренней переменной"},
                {"TAplMCEventStatusChangeLog_AlarmNonMaskedInputAct", "Тревога на немаскируемом входе"},
                {"TAplSCEvContrStatus_Offline", "Нет связи с контроллером"},
                {"TApcAuditEvent_Edit", "Аудит редактирования объекта"},
                {"TApcBolidEvent_AlarmReset", "Сброс тревоги"},
                {"TApcCardHolderAccess_DeniedReaderExclusList_ErrHolder", "Доступ запрещён, карта в списке исключений, владелец карты не найден"},
                {"TApcITVCam_Disarmed", "Камера ITV снята с охраны"},
                {"TAplSCEvStatusChange_InputAct", "Тревога на входе"},
                {"TApcAuditEvent_EditPerm", "Аудит редактирования прав/аудита объекта"},
                {"TApcCardHolderAccess_WillGranted_ErrHolder", "Будет доступ, владелец карты не найден"},
                {"TApcCardHolderCmdReq", "Выполнена команда со считывателя"},
                {"TApcBolidEvent_AlarmInputFault", "Ошибка параметров ШС"},
                {"TApcCardHolder_ResetAPB_ErrHolder", "Выполнена команда сброса статуса КПВ для карты, владелец карты не найден"},
                {"TApcITVCam_Armed", "Камера ITV поставлена на охрану"},
                {"TApcVEvContr_InterfaceOffline", "Нет связи с интерфейсным модулем VertX"},
                {"TApcVEvSmart_CustomCardData", "Считаны пользовательские данные смарт-карты"},
                {"TAplMCEventStatusChangeLog_SecureAllOk", "Вход на охране"},
                {"TApcCardHolderAccess_RealAPBViol", "Нарушение зонального КПВ"},
                {"TApcVEvSmart_ExecCmdLog", "Лог выполнения смарт-команды на считывателе"},
                {"TApcCardHolderAccess_DeniedBadIssueCode", "Доступ запрещён, ошибка версии карты"},
                {"TApcCardHolderAccess_DeniedPINUsed_ErrHolder", "Доступ запрещён, ПИН использован, владелец карты не найден"},
                {"TApcCardHolderAccess_GrantedUnderDuressNoEntry_ErrHolder", "Доступ разрешён, вход под принуждением, проход не осуществлён, владелец карты не найден"},
                {"TApcVEvAccess_DenyAccCardPINUnknown", "Доступ запрещён, неизвестная карта или ПИН"},
                {"TAplSCEvStatusChange_InputActAndMasked", "Вход не готов к постановке на охрану"},
                {"TApcBolidEvent_AccessDenied", "Доступ отклонен"},
                {"TApcBolidEvent_LaunchASPT", "Пуск АСПТ"},
                {"TApcCardHolderAccess_DeniedBadPIN", "Доступ запрещён, неправильный ПИН код"},
                {"TAplMCEventContrStatus_OffLine", "Нет связи с основным контроллером"},
                {"TApcBolidEvent_EntryAlarm", "Тревога проникновения"},
                {"TApcVISCam_StopMotion", "Прекратилось движение на камере NVR"},
                {"TAplMCEventPanelStatus_PowerUp", "Панель включёна"},
                {"TApcAuditEvent_RegEvent", "Аудит регистрации сообщения от объекта"},
                {"TApcVEvSmart_CardDetect", "Карта в области действия считывателя"},
                {"TApcVISCam_OnLine", "Соединение с камерой NVR установлено"},
                {"TAplMCEventZoneGroupChange", "Контроллер изменил уровень маскирования группы зон"},
                {"TApcCardHolderAccess_DeniedBiometricVerif_ErrHolder", "Доступ запрещён, неправильные биометрические данные, владелец карты не найден"},
                {"TApcCardHolderAccess_DeniedCmdAuthority_ErrHolder", "Доступ запрещён, нет прав на выполнение команды, владелец карты не найден"},
                {"TApcCardHolderAccess_GrantedNoEntry", "Доступ разрешён, проход не осуществлён"},
                {"TApcCardHolderAccess_GrantedVisitAccDuress", "Доступ посетителю разрешён, вход под принуждением"},
                {"TApcCardHolderAccess_TimedAPBViol", "Нарушение временного КПВ"},
                {"TApcMirasysCam_LinkRec", "Видеофрагмент с камеры Mirasys связан с сообщением"},
                {"TApcBolidEvent_OpenLine", "Обрыв цепи"},
                {"TApcBolidEvent_SilentAlarm", "Тихая тревога"},
                {"TApcCardHolderAccess_DeniedActZoneInGroup_ErrHolder", "Доступ запрещён, активная зона в группе, владелец карты не найден"},
                {"TApcCardHolderAccess_GrantedEscortAcc", "Доступ сопровождающему разрешён"},
                {"TApcEvAccordInput_Failed", "Нарушение входа"},
                {"TApcITVCamZone_AlarmReset", "Сброс тревоги на зоне детектора ITV"},
                {"TApcITVCam_Disable", "Камера ITV не доступна"},
                {"TAplMCEventStatusChangeLog_SecureInputActAndMasked", "Вход не готов к постановке"},
                {"TApcBolidEvent_ResetWatchdogTimer", "Сброс сторожевого таймера"},
                {"TAplMCEventAccReqNoCardLog_Diddle", "Попытка подбора кода"},
                {"TApcEvAccordInput_Warning", "Внимание на входе"},
                {"TApcITVCamZone_Armed", "Зона детектора ITV поставлена на охрану"},
                {"TApcBolidEvent_AutoDisabled", "Автоматика отключена"},
                {"TApcCardHolderAccess_DeniedBadIssueCode_ErrHolder", "Доступ запрещён, ошибка версии карты, владелец карты не найден"},
                {"TApcCardHolderAccess_DeniedDuress_ErrHolder", "Доступ запрещён, вход под принуждением, владелец карты не найден"},
                {"TApcCardHolderAccess_GrantedAPBViolDoorNotUsed_ErrHolder", "Доступ разрешён, ошибка КПВ, проход не осуществлён, владелец карты не найден"},
                {"TApcEvAccord512Contr_Offline", "БСПКА не на связи"},
                {"TAplMCEventAccReqNoCardLog_DeniedCardFrmtError", "Доступ запрещён, ошибка формата карты"},
                {"TAplMCEventStatusChangeLog_FaultGroudedLoop", "Вход заземлён"},
                {"TAplSCEvStatusChange_FaultOpenLine", "Обрыв на входе"},
                {"TApcCardHolderAccess_GrantedManual", "Доступ разрешён вручную"},
                {"TApcCardHolderAccess_GrantedVisitAcc_ErrHolder", "Доступ посетителю разрешён, владелец карты не найден"},
                {"TApcHolderAccessEvent", "Сообщение доступа"},
                {"TApcITVCam_Attached", "Камера ITV подключена"},
                {"TApcMirasysSocketDrv_OnLine", "Есть связь с сервером Mirasys"},
                {"TAplMCEventLEDAck", "Лампочка ASA-72 подтверждена"},
                {"TAplMCEventStatusChangeLog_SecureInputMasked", "Вход готов к постановке"},
                {"TAplSCEvStatusChange_InputMasked", "Вход готов к постановке на охрану"},
                {"TApcBolidEvent_AlertTech", "Нарушение технологического шлейфа"},
                {"TApcCardHolderAccess_DeniedBadAccLev_ErrHolder", "Доступ запрещён, ошибка уровня доступа, владелец карты не найден"},
                {"TApcCardHolderAccess_DeniedVisitNoEscort_ErrHolder", "Доступ посетителю запрещён, нет сопровождающего, владелец карты не найден"},
                {"TApcCardHolderAccess_GrantedEscortAccDuressNoEntry_ErrHolder", "Доступ сопровождающему разрешён, вход под принуждением, проход не осуществлён, владелец карты не найден"},
                {"TApcCardHolderAccess_GrantedEscortAccNoEntry", "Доступ сопровождающему разрешён, проход не осуществлён"},
                {"TApcCardHolderCmdReq_ErrHolder", "Выполнена команда со считывателя, владелец карты не найден"},
                {"TApcCardHolderHostAccReq_Granted_ErrHolder", "Доступ разрешен с компьютера, владелец карты не найден"},
                {"TApcITVSocketDrv_OnLine", "Есть связь с сервером ITV"},
                {"TApcTAAllEvent", "Сообщение УРВ (стандартная информация)"},
                {"TApcCardHolderAccess_DeniedNonEscortedVisitWhileVisit", "Доступ запрещен, предъявлена гостевая карта"},
                {"TApcCardHolderAccess_GrantedAPBViolDoorNotUsed", "Доступ разрешён, ошибка КПВ, проход не осуществлён"},
                {"TApcBolidEvent_FaultRCS", "Отказ СДУ"},
                {"TApcBolidEvent_InputEnabled", "ШС подключен"},
                {"TApcBolidEvent_RestoreDisarmedZone", "Восстановление снятой зоны"},
                {"TApcCardHolderAccess_DeniedNotInFile_ErrHolder", "Доступ запрещён, карта неизвестна контроллеру, владелец карты не найден"},
                {"TApcCardHolderAccess_GrantedVisitAccAPBViolDoorUsed_ErrHolder", "Доступ посетителю разрешён, ошибка КПВ, проход осуществлён, владелец карты не найден"},
                {"TApcSEContainer", "Системный контейнер"},
                {"TAplMCEventPowerUp", "Основной контроллер включён"},
                {"TApcAuditEvent_View", "Аудит просмотра объекта"},
                {"TApcCardHolderAccess_DeniedPINUsed", "Доступ запрещён, ПИН использован"},
                {"TAplMCEventTimeAdjustment_Forward", "Контроллер перешел на летнее время"},
                {"TApcBolidEvent_ShortedLine", "Короткое замыкание"},
                {"TApcCardHolderAccess_GrantedVisitAccAPBViolDoorNotUsed_ErrHolder", "Доступ посетителю разрешён, ошибка КПВ, проход не осуществлён, владелец карты не найден"},
                {"TApcEventLogout", "Завершение сеанса"},
                {"TApcITVCam_StopRec", "Завершена запись видео на камере ITV"},
                {"TApcCardHolderAccess_GrantedOnlyFC", "Доступ разрешён, проанализирован только код организации"},
                {"TAplMCEventZoneAPBStatus_Closed", "Контроллер закрыл зону КПВ"},
                {"TApcBolidEvent_Fire", "Пожар"},
                {"TApcCardHolderAccess_DeniedTimeOutNo2Card", "Доступ запрещён, не предъявлена вторая карта"},
                {"TApcEvAccordExtBlock_Fault", "Авария на блоке-расширителе"},
                {"TApcBolidEvent_RequireService", "Извещатель требует обслуживания"},
                {"TApcVISCam_OffLine", "Соединение с камерой NVR потеряно"},
                {"TApcBolidEvent_AutoIsEnabled", "Автоматика включена"},
                {"TApcCardHolderAccess_DeniedVisitWrongVisitGroup_ErrHolder", "Доступ посетителю запрещён, ошибка группы посетителей, владелец карты не найден"},
                {"TApcISSCam_Armed", "Камера ISS поставлена на охрану"},
                {"TAplMCEventZoneAPBStatus_Open", "Контроллер открыл зону КПВ"},
                {"TApcBolidEvent_ForcedOpen", "Дверь взломана"},
                {"TApcCardHolderAccess_GrantedEscortAccAPBViolDoorUsed_ErrHolder", "Доступ сопровождающему разрешён, ошибка КПВ, проход осуществлён, владелец карты не найден"},
                {"TApcCardHolderAccess_GrantedOnlyFC_ErrHolder", "Доступ разрешён, проанализирован только код организации, владелец карты не найден"},
                {"TAplMCEventStatusChangeLog_CommLineError", "Коммуникационная ошибка"},
                {"TApcBolidEvent_ZoneArmed", "Взятие зоны под охрану"},
                {"TApcCardHolderAccess_Correction", "Коррекция"},
                {"TApcCardHolderAccess_Granted", "Доступ разрешён"},
                {"TApcCardHolderAccess_DeniedAreaClosed", "Доступ запрещён, зона КПВ закрыта"},
                {"TApcCardHolderAccess_GrantedEscortAccAPBViolDoorUsed", "Доступ сопровождающему разрешён, ошибка КПВ, проход осуществлён"},
                {"TApcISSCam_Detached", "Камера ISS отключена"},
                {"TApcBolidEvent_ArmTech", "Восстановление технологического шлейфа"},
                {"TApcCardHolderAccess_DeniedCmdAuthority", "Доступ запрещён, нет прав на выполнение команды"},
                {"TApcCardHolderHostAccReq_Denied_ErrHolder", "Доступ запрещен с компьютера, владелец карты не найден"},
                {"TApcBolidEventXMLRPCServerStatus_OffLine", "Нет связи с Сервером Орион"},
                {"TApcBolidEvent_AccessOpen", "Доступ открыт"},
                {"TApcBolidEvent_IdentifyEO", "Идентификация хозоргана"},
                {"TApcBolidEvent_ZoneDisarmed", "Снятие шлейфа"},
                {"TApcCardHolderAccess_DeniedDuress", "Доступ запрещён, вход под принуждением"},
                {"TApcCardHolderHostAccReq_Granted", "Доступ разрешен с компьютера"},
                {"TApcVEvInput_Alarm", "Тревога на входе"},
                {"TApcVEvResetContrAPB", "Сброшен статус КПВ всех карт на контроллере"},
                {"TAplSCEvAccReqFC_DeniedFC", "Доступ запрещён, неверный код организации"},
                {"TApcBolidEvent_Online", "Прибор в норме"},
                {"TAplSCEvStatusChange_ShortedLine", "Вход закорочен"},
                {"TApcBolidEvent_PauseAutoStart", "Задержка пуска АСПТ"},
                {"TAplMCEventCardsDownload_Completed", "Загрузка карт закончена"},
                {"TAplMCEventStatusChangeLog_AlarmInputAct", "Тревога на входе"},
                {"TApcBolidEvent_Disarming", "Идет снятие"},
                {"TApcCardHolderAccess_GrantedOnlyFCNoEntry", "Доступ разрешён, проанализирован только код организации, проход не осуществлён"},
                {"TApcISSCam_Alarm", "Тревога на камере ISS"},
                {"TApcVEvContrStatus_Offline", "Нет связи с контроллером VertX"},
                {"TApcVEvContrStatus_Online", "Есть связь с контроллером VertX"},
                {"TApcVEvContr_InterfaceOnline", "Есть связь с интерфейсным модулем VertX"},
                {"TAplMCEventStatusChangeLog_NotConf", "Вход не конфигурирован"},
                {"TAplSCEvRelayChange", "Контроллером изменён режим реле"},
                {"TApcITVCam_Detached", "Камера ITV отключена"},
                {"TApcBolidEvent_RestorePowerSupply", "Восстановление источника питания"},
                {"TApcCardHolderAccess_GrantedNoEntry_ErrHolder", "Доступ разрешён, проход не осуществлён, владелец карты не найден"},
                {"TApcISSCam_StopRec", "Завершена запись видео на камере ISS"},
                {"TApcITVSocketDrv_OffLine", "Нет связи с сервером ITV"},
                {"TApcVEvAccess_HostLookup", "Поиск хоста"},
                {"TApcVEvInput_OffNormal", "Неисправность на входе"},
                {"TApcCardHolderAccess_RealAPBViol_ErrHolder", "Нарушение зонального КПВ, владелец карты не найден"},
                {"TApcBolidEvent_Arming", "Идет взятие"},
                {"TApcBolidEvent_Disarmed", "Снят"},
                {"TApcCardHolderAccess_GrantedVisitAccAPBViolDoorNotUsed", "Доступ посетителю разрешён, ошибка КПВ, проход не осуществлён"},
                {"TApcCardHolderAccess_DeniedBadPIN_ErrHolder", "Доступ запрещён, неправильный ПИН код, владелец карты не найден"},
                {"TApcCardHolderAccess_DeniedExp_ActDate_ErrHolder", "Доступ запрещён, неправильный срок действия карты, владелец карты не найден"},
                {"TApcCardHolderHostAccReq_DeniedWrongTID_ErrHolder", "Ошибка доступа, неверный идентификатор транзакции, владелец карты не найден"},
                {"TApcBolidEvent_AccessRestored", "Восстановление доступа"},
                {"TApcBolidEvent_FireEquipNorm", "Пожарное оборудование в норме"},
                {"TApcCardHolderAccess_DeniedActZoneInGroup", "Доступ запрещён, активная зона в группе"},
                {"TApcCardHolderAccess_GrantedEscortAccAPBViolDoorNotUsed", "Доступ сопровождающему разрешён, ошибка КПВ, проход не осуществлён"},
                {"TApcEventAck", "Подтверждение сообщения"},
                {"TApcISSCam_Attached", "Камера ISS подключена"},
                {"TApcITVCam_Alarm", "Тревога на камере ITV"},
                {"TApcVEvInput_Normal", "Вход в норме"},
                {"TAplSCEvReaderChange", "Контроллером изменён режим считывателя"},
                {"TApcEvAccord512Contr_Online", "БСПКА на связи"},
                {"TApcITVCam_StartRec", "Начата запись видео на камере ITV"},
                {"TAplMCEventRelayModeChange", "Контроллером изменён режим реле"},
                {"TApcBadPrevSrvTerminationEvent", "Некорректное завершение сервера"},
                {"TApcISSCam_AlarmReset", "Сброс тревоги на камере ISS"},
                {"TAplMCEventTimeAdjustment_Backward", "Контроллер перешел на зимнее время"},
                {"TAplSCEvAccReqNoCard_FrmtErr", "Доступ запрещён, ошибка формата карты"},
                {"TAplSCEvContrStatus_Online", "Есть связь с контроллером"},
                {"TApcEvAccordInput_Ok", "Вход на охране"},
                {"TApcISSCam_Disable", "Камера ISS не доступна"},
                {"TAplMCEventCmdRes", "Результат выполнения команды"},
                {"TAplMCEventStatusChangeLog_CommLossWrongRpl", "Связь прервана, неправильный ответ"},
                {"TApcEventLogin_Ok", "Успешная авторизация оператора"},
                {"TApcBolidEvent_InputDisabled", "ШС отключен"},
                {"TApcEvAccordInput_MaskAlarm", "Тревога, вход снят с охраны"},
                {"TApcBolidEvent_AccessBlocked", "Запрет доступа"},
                {"TApcBolidEvent_DoorSecure", "Восстановление целостности двери"},
                {"TApcBolidEvent_NotReadyToArm", "Нарушение снятого ШС"},
                {"TApcCardHolderAccess_AutoCorrection", "Автоматическая коррекция"},
                {"TApcCardHolderAccess_DeniedAreaClosed_ErrHolder", "Доступ запрещён, зона КПВ закрыта, владелец карты не найден"},
                {"TApcVEvAccess_AreaViolExit", "Нарушение зоны, выход"},
                {"TAplMCEventAccReqNoCardLog_DeniedReqOnlyPinNoCard", "Доступ запрещён, неизвестный ПИН"},
                {"TApcCardHolder_ResetAPB", "Выполнена команда сброса статуса КПВ для карты"},
                {"TAplMCEventAccReqNoCardLog_EPBReqDoorNotUsed", "Доступ разрешен по кнопке выхода, проход не осуществлён"},
                {"TAplSCEvAccReqNoCard_EPBReqDoorNotUsed", "Доступ разрешен по кнопке выхода, проход не осуществлён"},
                {"TAplSCEvPowerUp", "Контроллер включён"},
                {"TApcAuditEvent_Delete", "Аудит удаления объекта"},
                {"TApcCardHolderAccess_DeniedBiometricVerif", "Доступ запрещён, неправильные биометрические данные"},
                {"TApcCardHolderAccess_GrantedEscortAcc_ErrHolder", "Доступ сопровождающему разрешён, владелец карты не найден"},
                {"TApcCardHolderAccess_GrantedUnderDuress", "Доступ разрешён, вход под принуждением"},
                {"TApcCardHolderAccess_GrantedUnderDuress_ErrHolder", "Доступ разрешён, вход под принуждением, владелец карты не найден"},
                {"TApcCardHolderAccess_DeniedTimeOutNo2Card_ErrHolder", "Доступ запрещён, не предъявлена вторая карта, владелец карты не найден"},
                {"TApcCardHolderAccess_GrantedEscortAccDuress_ErrHolder", "Доступ сопровождающему разрешён, вход под принуждением, владелец карты не найден"},
                {"TApcISSCam_LinkRec", "Видеофрагмент с камеры ISS связан с сообщением"},
                {"TAplMCEventStatusChangeLog_CommLossDeviceID", "Связь прервана, неправильный идентификатор устройства"},
                {"TApcBolidEvent_Offline", "Нет контакта"},
                {"TApcCardHolderAccess_GrantedOnlyFCNoEntry_ErrHolder", "Доступ разрешён, проанализирован только код организации, проход не осуществлён, владелец карты не найден"},
                {"TApcEvAccordInput_MaskOk", "Вход снят с охраны"},
                {"TApcBolidEvent_EmergStart", "Аварийный Пуск"},
                {"TApcCardHolderAccess_HostAccReq_ErrHolder", "Запрос доступа на компьютер, владелец карты не найден"},
                {"TApcTimeAttendanceEvent", "Сообщение УРВ"},
                {"TAplSCEvAccReqNoCard_EPBReqDoorUsed", "Доступ разрешен по кнопке выхода, проход осуществлён"},
                {"TAplSCEvStatusChange_FaultNonSpecific", "Ошибка на входе"},
                {"TApcDBFailedEvent", "Ошибка при работе с базой данных"},
                {"TApcISSSocketDrv_OnLine", "Есть связь с сервером ISS"},
                {"TApcVEvIOLinker_Custom", "Пользовательское сообщение VertX"},
                {"TAplMCEventHostAcc_GrantedNoCardAv", "Запущен цикл прохода"},
                {"TAplSCEvStatusChange_SecureAllOk", "Вход на охране"},
                {"TApcCardHolderAccess_DeniedBadAccLev", "Доступ запрещён, ошибка уровня доступа"},
                {"TApcVEvTask_Start", "Служба запущена"},
                {"TApcAuditEvent_Add", "Аудит добавления объекта"},
                {"TApcBolidEvent_ZoneNotArmed", "Ошибка постановки ШС под охрану"},
                {"TApcCardHolderAccess_DeniedManual", "Доступ запрещен вручную"},
                {"TApcEvAccordInput_MaskFailed", "Нарушение, вход снят с охраны"},
                {"TApcITVCam_AlarmReset", "Сброс тревоги на камере ITV"},
                {"TAplSCEvAccReqNoCard_DeniedOnlyPIN", "Доступ запрещён, неизвестный ПИН"}
            };
    }
}
