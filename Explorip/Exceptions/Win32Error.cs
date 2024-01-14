using System;

namespace BetterWin32Errors;

/// <summary>
/// Constants defined in winerror.h.
/// </summary>
public enum Win32Error : uint
{
    /// <summary>The operation completed successfully.</summary>
    ERROR_SUCCESS = 0,
    /// <summary>Incorrect function.</summary>
    ERROR_INVALID_FUNCTION = 1,    // dderror
    /// <summary>The system cannot find the file specified.</summary>
    ERROR_FILE_NOT_FOUND = 2,
    /// <summary>The system cannot find the path specified.</summary>
    ERROR_PATH_NOT_FOUND = 3,
    /// <summary>The system cannot open the file.</summary>
    ERROR_TOO_MANY_OPEN_FILES = 4,
    /// <summary>Access is denied.</summary>
    ERROR_ACCESS_DENIED = 5,
    /// <summary>The handle is invalid.</summary>
    ERROR_INVALID_HANDLE = 6,
    /// <summary>The storage control blocks were destroyed.</summary>
    ERROR_ARENA_TRASHED = 7,
    /// <summary>Not enough storage is available to process this command.</summary>
    ERROR_NOT_ENOUGH_MEMORY = 8,    // dderror
    /// <summary>The storage control block address is invalid.</summary>
    ERROR_INVALID_BLOCK = 9,
    /// <summary>The environment is incorrect.</summary>
    ERROR_BAD_ENVIRONMENT = 10,
    /// <summary>An attempt was made to load a program with an incorrect format.</summary>
    ERROR_BAD_FORMAT = 11,
    /// <summary>The access code is invalid.</summary>
    ERROR_INVALID_ACCESS = 12,
    /// <summary>The data is invalid.</summary>
    ERROR_INVALID_DATA = 13,
    /// <summary>Not enough storage is available to complete this operation.</summary>
    ERROR_OUTOFMEMORY = 14,
    /// <summary>The system cannot find the drive specified.</summary>
    ERROR_INVALID_DRIVE = 15,
    /// <summary>The directory cannot be removed.</summary>
    ERROR_CURRENT_DIRECTORY = 16,
    /// <summary>The system cannot move the file to a different disk drive.</summary>
    ERROR_NOT_SAME_DEVICE = 17,
    /// <summary>There are no more files.</summary>
    ERROR_NO_MORE_FILES = 18,
    /// <summary>The media is write protected.</summary>
    ERROR_WRITE_PROTECT = 19,
    /// <summary>The system cannot find the device specified.</summary>
    ERROR_BAD_UNIT = 20,
    /// <summary>The device is not ready.</summary>
    ERROR_NOT_READY = 21,
    /// <summary>The device does not recognize the command.</summary>
    ERROR_BAD_COMMAND = 22,
    /// <summary>Data error (cyclic redundancy check).</summary>
    ERROR_CRC = 23,
    /// <summary>The program issued a command but the command length is incorrect.</summary>
    ERROR_BAD_LENGTH = 24,
    /// <summary>The drive cannot locate a specific area or track on the disk.</summary>
    ERROR_SEEK = 25,
    /// <summary>The specified disk or diskette cannot be accessed.</summary>
    ERROR_NOT_DOS_DISK = 26,
    /// <summary>The drive cannot find the sector requested.</summary>
    ERROR_SECTOR_NOT_FOUND = 27,
    /// <summary>The printer is out of paper.</summary>
    ERROR_OUT_OF_PAPER = 28,
    /// <summary>The system cannot write to the specified device.</summary>
    ERROR_WRITE_FAULT = 29,
    /// <summary>The system cannot read from the specified device.</summary>
    ERROR_READ_FAULT = 30,
    /// <summary>A device attached to the system is not functioning.</summary>
    ERROR_GEN_FAILURE = 31,
    /// <summary>The process cannot access the file because it is being used by another process.</summary>
    ERROR_SHARING_VIOLATION = 32,
    /// <summary>The process cannot access the file because another process has locked a portion of the file.</summary>
    ERROR_LOCK_VIOLATION = 33,
    /// <summary>The wrong diskette is in the drive. Insert %2 (Volume Serial Number: %3) into drive %1.</summary>
    ERROR_WRONG_DISK = 34,
    /// <summary>Too many files opened for sharing.</summary>
    ERROR_SHARING_BUFFER_EXCEEDED = 36,
    /// <summary>Reached the end of the file.</summary>
    ERROR_HANDLE_EOF = 38,
    /// <summary>The disk is full.</summary>
    ERROR_HANDLE_DISK_FULL = 39,
    /// <summary>The request is not supported.</summary>
    ERROR_NOT_SUPPORTED = 50,
    /// <summary>Windows cannot find the network path. Verify that the network path is correct and the destination computer is not busy or turned off. If Windows still cannot find the network path, contact your network administrator.</summary>
    ERROR_REM_NOT_LIST = 51,
    /// <summary>You were not connected because a duplicate name exists on the network. If joining a domain, go to System in Control Panel to change the computer name and try again. If joining a workgroup, choose another workgroup name.</summary>
    ERROR_DUP_NAME = 52,
    /// <summary>The network path was not found.</summary>
    ERROR_BAD_NETPATH = 53,
    /// <summary>The network is busy.</summary>
    ERROR_NETWORK_BUSY = 54,
    /// <summary>The specified network resource or device is no longer available.</summary>
    ERROR_DEV_NOT_EXIST = 55,    // dderror
    /// <summary>The network BIOS command limit has been reached.</summary>
    ERROR_TOO_MANY_CMDS = 56,
    /// <summary>A network adapter hardware error occurred.</summary>
    ERROR_ADAP_HDW_ERR = 57,
    /// <summary>The specified server cannot perform the requested operation.</summary>
    ERROR_BAD_NET_RESP = 58,
    /// <summary>An unexpected network error occurred.</summary>
    ERROR_UNEXP_NET_ERR = 59,
    /// <summary>The remote adapter is not compatible.</summary>
    ERROR_BAD_REM_ADAP = 60,
    /// <summary>The printer queue is full.</summary>
    ERROR_PRINTQ_FULL = 61,
    /// <summary>Space to store the file waiting to be printed is not available on the server.</summary>
    ERROR_NO_SPOOL_SPACE = 62,
    /// <summary>Your file waiting to be printed was deleted.</summary>
    ERROR_PRINT_CANCELLED = 63,
    /// <summary>The specified network name is no longer available.</summary>
    ERROR_NETNAME_DELETED = 64,
    /// <summary>Network access is denied.</summary>
    ERROR_NETWORK_ACCESS_DENIED = 65,
    /// <summary>The network resource type is not correct.</summary>
    ERROR_BAD_DEV_TYPE = 66,
    /// <summary>The network name cannot be found.</summary>
    ERROR_BAD_NET_NAME = 67,
    /// <summary>The name limit for the local computer network adapter card was exceeded.</summary>
    ERROR_TOO_MANY_NAMES = 68,
    /// <summary>The network BIOS session limit was exceeded.</summary>
    ERROR_TOO_MANY_SESS = 69,
    /// <summary>The remote server has been paused or is in the process of being started.</summary>
    ERROR_SHARING_PAUSED = 70,
    /// <summary>No more connections can be made to this remote computer at this time because there are already as many connections as the computer can accept.</summary>
    ERROR_REQ_NOT_ACCEP = 71,
    /// <summary>The specified printer or disk device has been paused.</summary>
    ERROR_REDIR_PAUSED = 72,
    /// <summary>The file exists.</summary>
    ERROR_FILE_EXISTS = 80,
    /// <summary>The directory or file cannot be created.</summary>
    ERROR_CANNOT_MAKE = 82,
    /// <summary>Fail on INT 24.</summary>
    ERROR_FAIL_I24 = 83,
    /// <summary>Storage to process this request is not available.</summary>
    ERROR_OUT_OF_STRUCTURES = 84,
    /// <summary>The local device name is already in use.</summary>
    ERROR_ALREADY_ASSIGNED = 85,
    /// <summary>The specified network password is not correct.</summary>
    ERROR_INVALID_PASSWORD = 86,
    /// <summary>The parameter is incorrect.</summary>
    ERROR_INVALID_PARAMETER = 87,    // dderror
    /// <summary>A write fault occurred on the network.</summary>
    ERROR_NET_WRITE_FAULT = 88,
    /// <summary>The system cannot start another process at this time.</summary>
    ERROR_NO_PROC_SLOTS = 89,
    /// <summary>Cannot create another system semaphore.</summary>
    ERROR_TOO_MANY_SEMAPHORES = 100,
    /// <summary>The exclusive semaphore is owned by another process.</summary>
    ERROR_EXCL_SEM_ALREADY_OWNED = 101,
    /// <summary>The semaphore is set and cannot be closed.</summary>
    ERROR_SEM_IS_SET = 102,
    /// <summary>The semaphore cannot be set again.</summary>
    ERROR_TOO_MANY_SEM_REQUESTS = 103,
    /// <summary>Cannot request exclusive semaphores at interrupt time.</summary>
    ERROR_INVALID_AT_INTERRUPT_TIME = 104,
    /// <summary>The previous ownership of this semaphore has ended.</summary>
    ERROR_SEM_OWNER_DIED = 105,
    /// <summary>Insert the diskette for drive %1.</summary>
    ERROR_SEM_USER_LIMIT = 106,
    /// <summary>The program stopped because an alternate diskette was not inserted.</summary>
    ERROR_DISK_CHANGE = 107,
    /// <summary>The disk is in use or locked by another process.</summary>
    ERROR_DRIVE_LOCKED = 108,
    /// <summary>The pipe has been ended.</summary>
    ERROR_BROKEN_PIPE = 109,
    /// <summary>The system cannot open the device or file specified.</summary>
    ERROR_OPEN_FAILED = 110,
    /// <summary>The file name is too long.</summary>
    ERROR_BUFFER_OVERFLOW = 111,
    /// <summary>There is not enough space on the disk.</summary>
    ERROR_DISK_FULL = 112,
    /// <summary>No more internal file identifiers available.</summary>
    ERROR_NO_MORE_SEARCH_HANDLES = 113,
    /// <summary>The target internal file identifier is incorrect.</summary>
    ERROR_INVALID_TARGET_HANDLE = 114,
    /// <summary>The IOCTL call made by the application program is not correct.</summary>
    ERROR_INVALID_CATEGORY = 117,
    /// <summary>The verify-on-write switch parameter value is not correct.</summary>
    ERROR_INVALID_VERIFY_SWITCH = 118,
    /// <summary>The system does not support the command requested.</summary>
    ERROR_BAD_DRIVER_LEVEL = 119,
    /// <summary>This function is not supported on this system.</summary>
    ERROR_CALL_NOT_IMPLEMENTED = 120,
    /// <summary>The semaphore timeout period has expired.</summary>
    ERROR_SEM_TIMEOUT = 121,
    /// <summary>The data area passed to a system call is too small.</summary>
    ERROR_INSUFFICIENT_BUFFER = 122,    // dderror
    /// <summary>The filename, directory name, or volume label syntax is incorrect.</summary>
    ERROR_INVALID_NAME = 123,    // dderror
    /// <summary>The system call level is not correct.</summary>
    ERROR_INVALID_LEVEL = 124,
    /// <summary>The disk has no volume label.</summary>
    ERROR_NO_VOLUME_LABEL = 125,
    /// <summary>The specified module could not be found.</summary>
    ERROR_MOD_NOT_FOUND = 126,
    /// <summary>The specified procedure could not be found.</summary>
    ERROR_PROC_NOT_FOUND = 127,
    /// <summary>There are no child processes to wait for.</summary>
    ERROR_WAIT_NO_CHILDREN = 128,
    /// <summary>The %1 application cannot be run in Win32 mode.</summary>
    ERROR_CHILD_NOT_COMPLETE = 129,
    /// <summary>Attempt to use a file handle to an open disk partition for an operation other than raw disk I/O.</summary>
    ERROR_DIRECT_ACCESS_HANDLE = 130,
    /// <summary>An attempt was made to move the file pointer before the beginning of the file.</summary>
    ERROR_NEGATIVE_SEEK = 131,
    /// <summary>The file pointer cannot be set on the specified device or file.</summary>
    ERROR_SEEK_ON_DEVICE = 132,
    /// <summary>A JOIN or SUBST command cannot be used for a drive that contains previously joined drives.</summary>
    ERROR_IS_JOIN_TARGET = 133,
    /// <summary>An attempt was made to use a JOIN or SUBST command on a drive that has already been joined.</summary>
    ERROR_IS_JOINED = 134,
    /// <summary>An attempt was made to use a JOIN or SUBST command on a drive that has already been substituted.</summary>
    ERROR_IS_SUBSTED = 135,
    /// <summary>The system tried to delete the JOIN of a drive that is not joined.</summary>
    ERROR_NOT_JOINED = 136,
    /// <summary>The system tried to delete the substitution of a drive that is not substituted.</summary>
    ERROR_NOT_SUBSTED = 137,
    /// <summary>The system tried to join a drive to a directory on a joined drive.</summary>
    ERROR_JOIN_TO_JOIN = 138,
    /// <summary>The system tried to substitute a drive to a directory on a substituted drive.</summary>
    ERROR_SUBST_TO_SUBST = 139,
    /// <summary>The system tried to join a drive to a directory on a substituted drive.</summary>
    ERROR_JOIN_TO_SUBST = 140,
    /// <summary>The system tried to SUBST a drive to a directory on a joined drive.</summary>
    ERROR_SUBST_TO_JOIN = 141,
    /// <summary>The system cannot perform a JOIN or SUBST at this time.</summary>
    ERROR_BUSY_DRIVE = 142,
    /// <summary>The system cannot join or substitute a drive to or for a directory on the same drive.</summary>
    ERROR_SAME_DRIVE = 143,
    /// <summary>The directory is not a subdirectory of the root directory.</summary>
    ERROR_DIR_NOT_ROOT = 144,
    /// <summary>The directory is not empty.</summary>
    ERROR_DIR_NOT_EMPTY = 145,
    /// <summary>The path specified is being used in a substitute.</summary>
    ERROR_IS_SUBST_PATH = 146,
    /// <summary>Not enough resources are available to process this command.</summary>
    ERROR_IS_JOIN_PATH = 147,
    /// <summary>The path specified cannot be used at this time.</summary>
    ERROR_PATH_BUSY = 148,
    /// <summary>An attempt was made to join or substitute a drive for which a directory on the drive is the target of a previous substitute.</summary>
    ERROR_IS_SUBST_TARGET = 149,
    /// <summary>System trace information was not specified in your CONFIG.SYS file, or tracing is disallowed.</summary>
    ERROR_SYSTEM_TRACE = 150,
    /// <summary>The number of specified semaphore events for DosMuxSemWait is not correct.</summary>
    ERROR_INVALID_EVENT_COUNT = 151,
    /// <summary>DosMuxSemWait did not execute; too many semaphores are already set.</summary>
    ERROR_TOO_MANY_MUXWAITERS = 152,
    /// <summary>The DosMuxSemWait list is not correct.</summary>
    ERROR_INVALID_LIST_FORMAT = 153,
    /// <summary>The volume label you entered exceeds the label character limit of the target file system.</summary>
    ERROR_LABEL_TOO_LONG = 154,
    /// <summary>Cannot create another thread.</summary>
    ERROR_TOO_MANY_TCBS = 155,
    /// <summary>The recipient process has refused the signal.</summary>
    ERROR_SIGNAL_REFUSED = 156,
    /// <summary>The segment is already discarded and cannot be locked.</summary>
    ERROR_DISCARDED = 157,
    /// <summary>The segment is already unlocked.</summary>
    ERROR_NOT_LOCKED = 158,
    /// <summary>The address for the thread ID is not correct.</summary>
    ERROR_BAD_THREADID_ADDR = 159,
    /// <summary>One or more arguments are not correct.</summary>
    ERROR_BAD_ARGUMENTS = 160,
    /// <summary>The specified path is invalid.</summary>
    ERROR_BAD_PATHNAME = 161,
    /// <summary>A signal is already pending.</summary>
    ERROR_SIGNAL_PENDING = 162,
    /// <summary>No more threads can be created in the system.</summary>
    ERROR_MAX_THRDS_REACHED = 164,
    /// <summary>Unable to lock a region of a file.</summary>
    ERROR_LOCK_FAILED = 167,
    /// <summary>The requested resource is in use.</summary>
    ERROR_BUSY = 170,    // dderror
    /// <summary>A lock request was not outstanding for the supplied cancel region.</summary>
    ERROR_CANCEL_VIOLATION = 173,
    /// <summary>The file system does not support atomic changes to the lock type.</summary>
    ERROR_ATOMIC_LOCKS_NOT_SUPPORTED = 174,
    /// <summary>The system detected a segment number that was not correct.</summary>
    ERROR_INVALID_SEGMENT_NUMBER = 180,
    /// <summary>The operating system cannot run %1.</summary>
    ERROR_INVALID_ORDINAL = 182,
    /// <summary>Cannot create a file when that file already exists.</summary>
    ERROR_ALREADY_EXISTS = 183,
    /// <summary>The flag passed is not correct.</summary>
    ERROR_INVALID_FLAG_NUMBER = 186,
    /// <summary>The specified system semaphore name was not found.</summary>
    ERROR_SEM_NOT_FOUND = 187,
    /// <summary>The operating system cannot run %1.</summary>
    ERROR_INVALID_STARTING_CODESEG = 188,
    /// <summary>The operating system cannot run %1.</summary>
    ERROR_INVALID_STACKSEG = 189,
    /// <summary>The operating system cannot run %1.</summary>
    ERROR_INVALID_MODULETYPE = 190,
    /// <summary>Cannot run %1 in Win32 mode.</summary>
    ERROR_INVALID_EXE_SIGNATURE = 191,
    /// <summary>The operating system cannot run %1.</summary>
    ERROR_EXE_MARKED_INVALID = 192,
    /// <summary>%1 is not a valid Win32 application.</summary>
    ERROR_BAD_EXE_FORMAT = 193,
    /// <summary>The operating system cannot run %1.</summary>
    ERROR_ITERATED_DATA_EXCEEDS_64k = 194,
    /// <summary>The operating system cannot run %1.</summary>
    ERROR_INVALID_MINALLOCSIZE = 195,
    /// <summary>The operating system cannot run this application program.</summary>
    ERROR_DYNLINK_FROM_INVALID_RING = 196,
    /// <summary>The operating system is not presently configured to run this application.</summary>
    ERROR_IOPL_NOT_ENABLED = 197,
    /// <summary>The operating system cannot run %1.</summary>
    ERROR_INVALID_SEGDPL = 198,
    /// <summary>The operating system cannot run this application program.</summary>
    ERROR_AUTODATASEG_EXCEEDS_64k = 199,
    /// <summary>The code segment cannot be greater than or equal to 64K.</summary>
    ERROR_RING2SEG_MUST_BE_MOVABLE = 200,
    /// <summary>The operating system cannot run %1.</summary>
    ERROR_RELOC_CHAIN_XEEDS_SEGLIM = 201,
    /// <summary>The operating system cannot run %1.</summary>
    ERROR_INFLOOP_IN_RELOC_CHAIN = 202,
    /// <summary>The system could not find the environment option that was entered.</summary>
    ERROR_ENVVAR_NOT_FOUND = 203,
    /// <summary>No process in the command subtree has a signal handler.</summary>
    ERROR_NO_SIGNAL_SENT = 205,
    /// <summary>The filename or extension is too long.</summary>
    ERROR_FILENAME_EXCED_RANGE = 206,
    /// <summary>The ring 2 stack is in use.</summary>
    ERROR_RING2_STACK_IN_USE = 207,
    /// <summary>The global filename characters, * or ?, are entered incorrectly or too many global filename characters are specified.</summary>
    ERROR_META_EXPANSION_TOO_LONG = 208,
    /// <summary>The signal being posted is not correct.</summary>
    ERROR_INVALID_SIGNAL_NUMBER = 209,
    /// <summary>The signal handler cannot be set.</summary>
    ERROR_THREAD_1_INACTIVE = 210,
    /// <summary>The segment is locked and cannot be reallocated.</summary>
    ERROR_LOCKED = 212,
    /// <summary>Too many dynamic-link modules are attached to this program or dynamic-link module.</summary>
    ERROR_TOO_MANY_MODULES = 214,
    /// <summary>Cannot nest calls to LoadModule.</summary>
    ERROR_NESTING_NOT_ALLOWED = 215,
    /// <summary>This version of %1 is not compatible with the version of Windows you&#39;re running. Check your computer&#39;s system information to see whether you need a x86 (32-bit) or x64 (64-bit) version of the program, and then contact the software publisher.</summary>
    ERROR_EXE_MACHINE_TYPE_MISMATCH = 216,
    /// <summary>The image file %1 is signed, unable to modify.</summary>
    ERROR_EXE_CANNOT_MODIFY_SIGNED_BINARY = 217,
    /// <summary>The image file %1 is strong signed, unable to modify.</summary>
    ERROR_EXE_CANNOT_MODIFY_STRONG_SIGNED_BINARY = 218,
    /// <summary>This file is checked out or locked for editing by another user.</summary>
    ERROR_FILE_CHECKED_OUT = 220,
    /// <summary>The file must be checked out before saving changes.</summary>
    ERROR_CHECKOUT_REQUIRED = 221,
    /// <summary>The file type being saved or retrieved has been blocked.</summary>
    ERROR_BAD_FILE_TYPE = 222,
    /// <summary>The file size exceeds the limit allowed and cannot be saved.</summary>
    ERROR_FILE_TOO_LARGE = 223,
    /// <summary>Access Denied. Before opening files in this location, you must first add the web site to your trusted sites list, browse to the web site, and select the option to login automatically.</summary>
    ERROR_FORMS_AUTH_REQUIRED = 224,
    /// <summary>Operation did not complete successfully because the file contains a virus.</summary>
    ERROR_VIRUS_INFECTED = 225,
    /// <summary>This file contains a virus and cannot be opened. Due to the nature of this virus, the file has been removed from this location.</summary>
    ERROR_VIRUS_DELETED = 226,
    /// <summary>The pipe is local.</summary>
    ERROR_PIPE_LOCAL = 229,
    /// <summary>The pipe state is invalid.</summary>
    ERROR_BAD_PIPE = 230,
    /// <summary>All pipe instances are busy.</summary>
    ERROR_PIPE_BUSY = 231,
    /// <summary>The pipe is being closed.</summary>
    ERROR_NO_DATA = 232,
    /// <summary>No process is on the other end of the pipe.</summary>
    ERROR_PIPE_NOT_CONNECTED = 233,
    /// <summary>More data is available.</summary>
    ERROR_MORE_DATA = 234,    // dderror
    /// <summary>The session was canceled.</summary>
    ERROR_VC_DISCONNECTED = 240,
    /// <summary>The specified extended attribute name was invalid.</summary>
    ERROR_INVALID_EA_NAME = 254,
    /// <summary>The extended attributes are inconsistent.</summary>
    ERROR_EA_LIST_INCONSISTENT = 255,
    /// <summary>The wait operation timed out.</summary>
    WAIT_TIMEOUT = 258,    // dderror
    /// <summary>No more data is available.</summary>
    ERROR_NO_MORE_ITEMS = 259,
    /// <summary>The copy functions cannot be used.</summary>
    ERROR_CANNOT_COPY = 266,
    /// <summary>The directory name is invalid.</summary>
    ERROR_DIRECTORY = 267,
    /// <summary>The extended attributes did not fit in the buffer.</summary>
    ERROR_EAS_DIDNT_FIT = 275,
    /// <summary>The extended attribute file on the mounted file system is corrupt.</summary>
    ERROR_EA_FILE_CORRUPT = 276,
    /// <summary>The extended attribute table file is full.</summary>
    ERROR_EA_TABLE_FULL = 277,
    /// <summary>The specified extended attribute handle is invalid.</summary>
    ERROR_INVALID_EA_HANDLE = 278,
    /// <summary>The mounted file system does not support extended attributes.</summary>
    ERROR_EAS_NOT_SUPPORTED = 282,
    /// <summary>Attempt to release mutex not owned by caller.</summary>
    ERROR_NOT_OWNER = 288,
    /// <summary>Too many posts were made to a semaphore.</summary>
    ERROR_TOO_MANY_POSTS = 298,
    /// <summary>Only part of a ReadProcessMemory or WriteProcessMemory request was completed.</summary>
    ERROR_PARTIAL_COPY = 299,
    /// <summary>The oplock request is denied.</summary>
    ERROR_OPLOCK_NOT_GRANTED = 300,
    /// <summary>An invalid oplock acknowledgment was received by the system.</summary>
    ERROR_INVALID_OPLOCK_PROTOCOL = 301,
    /// <summary>The volume is too fragmented to complete this operation.</summary>
    ERROR_DISK_TOO_FRAGMENTED = 302,
    /// <summary>The file cannot be opened because it is in the process of being deleted.</summary>
    ERROR_DELETE_PENDING = 303,
    /// <summary>Short name settings may not be changed on this volume due to the global registry setting.</summary>
    ERROR_INCOMPATIBLE_WITH_GLOBAL_SHORT_NAME_REGISTRY_SETTING = 304,
    /// <summary>Short names are not enabled on this volume.</summary>
    ERROR_SHORT_NAMES_NOT_ENABLED_ON_VOLUME = 305,
    /// <summary>The security stream for the given volume is in an inconsistent state. Please run CHKDSK on the volume.</summary>
    ERROR_SECURITY_STREAM_IS_INCONSISTENT = 306,
    /// <summary>A requested file lock operation cannot be processed due to an invalid byte range.</summary>
    ERROR_INVALID_LOCK_RANGE = 307,
    /// <summary>The subsystem needed to support the image type is not present.</summary>
    ERROR_IMAGE_SUBSYSTEM_NOT_PRESENT = 308,
    /// <summary>The specified file already has a notification GUID associated with it.</summary>
    ERROR_NOTIFICATION_GUID_ALREADY_DEFINED = 309,
    /// <summary>The system cannot find message text for message number 0x%1 in the message file for %2.</summary>
    ERROR_MR_MID_NOT_FOUND = 317,
    /// <summary>The scope specified was not found.</summary>
    ERROR_SCOPE_NOT_FOUND = 318,
    /// <summary>No action was taken as a system reboot is required.</summary>
    ERROR_FAIL_NOACTION_REBOOT = 350,
    /// <summary>The shutdown operation failed.</summary>
    ERROR_FAIL_SHUTDOWN = 351,
    /// <summary>The restart operation failed.</summary>
    ERROR_FAIL_RESTART = 352,
    /// <summary>The maximum number of sessions has been reached.</summary>
    ERROR_MAX_SESSIONS_REACHED = 353,
    /// <summary>The thread is already in background processing mode.</summary>
    ERROR_THREAD_MODE_ALREADY_BACKGROUND = 400,
    /// <summary>The thread is not in background processing mode.</summary>
    ERROR_THREAD_MODE_NOT_BACKGROUND = 401,
    /// <summary>The process is already in background processing mode.</summary>
    ERROR_PROCESS_MODE_ALREADY_BACKGROUND = 402,
    /// <summary>The process is not in background processing mode.</summary>
    ERROR_PROCESS_MODE_NOT_BACKGROUND = 403,
    /// <summary>Attempt to access invalid address.</summary>
    ERROR_INVALID_ADDRESS = 487,
    /// <summary>User profile cannot be loaded.</summary>
    ERROR_USER_PROFILE_LOAD = 500,
    /// <summary>Arithmetic result exceeded 32 bits.</summary>
    ERROR_ARITHMETIC_OVERFLOW = 534,
    /// <summary>There is a process on other end of the pipe.</summary>
    ERROR_PIPE_CONNECTED = 535,
    /// <summary>Waiting for a process to open the other end of the pipe.</summary>
    ERROR_PIPE_LISTENING = 536,
    /// <summary>Application verifier has found an error in the current process.</summary>
    ERROR_VERIFIER_STOP = 537,
    /// <summary>An error occurred in the ABIOS subsystem.</summary>
    ERROR_ABIOS_ERROR = 538,
    /// <summary>A warning occurred in the WX86 subsystem.</summary>
    ERROR_WX86_WARNING = 539,
    /// <summary>An error occurred in the WX86 subsystem.</summary>
    ERROR_WX86_ERROR = 540,
    /// <summary>An attempt was made to cancel or set a timer that has an associated APC and the subject thread is not the thread that originally set the timer with an associated APC routine.</summary>
    ERROR_TIMER_NOT_CANCELED = 541,
    /// <summary>Unwind exception code.</summary>
    ERROR_UNWIND = 542,
    /// <summary>An invalid or unaligned stack was encountered during an unwind operation.</summary>
    ERROR_BAD_STACK = 543,
    /// <summary>An invalid unwind target was encountered during an unwind operation.</summary>
    ERROR_INVALID_UNWIND_TARGET = 544,
    /// <summary>Invalid Object Attributes specified to NtCreatePort or invalid Port Attributes specified to NtConnectPort</summary>
    ERROR_INVALID_PORT_ATTRIBUTES = 545,
    /// <summary>Length of message passed to NtRequestPort or NtRequestWaitReplyPort was longer than the maximum message allowed by the port.</summary>
    ERROR_PORT_MESSAGE_TOO_LONG = 546,
    /// <summary>An attempt was made to lower a quota limit below the current usage.</summary>
    ERROR_INVALID_QUOTA_LOWER = 547,
    /// <summary>An attempt was made to attach to a device that was already attached to another device.</summary>
    ERROR_DEVICE_ALREADY_ATTACHED = 548,
    /// <summary>An attempt was made to execute an instruction at an unaligned address and the host system does not support unaligned instruction references.</summary>
    ERROR_INSTRUCTION_MISALIGNMENT = 549,
    /// <summary>Profiling not started.</summary>
    ERROR_PROFILING_NOT_STARTED = 550,
    /// <summary>Profiling not stopped.</summary>
    ERROR_PROFILING_NOT_STOPPED = 551,
    /// <summary>The passed ACL did not contain the minimum required information.</summary>
    ERROR_COULD_NOT_INTERPRET = 552,
    /// <summary>The number of active profiling objects is at the maximum and no more may be started.</summary>
    ERROR_PROFILING_AT_LIMIT = 553,
    /// <summary>Used to indicate that an operation cannot continue without blocking for I/O.</summary>
    ERROR_CANT_WAIT = 554,
    /// <summary>Indicates that a thread attempted to terminate itself by default (called NtTerminateThread with NULL) and it was the last thread in the current process.</summary>
    ERROR_CANT_TERMINATE_SELF = 555,
    /// <summary>If an MM error is returned which is not defined in the standard FsRtl filter, it is converted to one of the following errors which is guaranteed to be in the filter. In this case information is lost, however, the filter correctly handles the exception.</summary>
    ERROR_UNEXPECTED_MM_CREATE_ERR = 556,
    /// <summary>If an MM error is returned which is not defined in the standard FsRtl filter, it is converted to one of the following errors which is guaranteed to be in the filter. In this case information is lost, however, the filter correctly handles the exception.</summary>
    ERROR_UNEXPECTED_MM_MAP_ERROR = 557,
    /// <summary>If an MM error is returned which is not defined in the standard FsRtl filter, it is converted to one of the following errors which is guaranteed to be in the filter. In this case information is lost, however, the filter correctly handles the exception.</summary>
    ERROR_UNEXPECTED_MM_EXTEND_ERR = 558,
    /// <summary>A malformed function table was encountered during an unwind operation.</summary>
    ERROR_BAD_FUNCTION_TABLE = 559,
    /// <summary>Indicates that an attempt was made to assign protection to a file system file or directory and one of the SIDs in the security descriptor could not be translated into a GUID that could be stored by the file system. This causes the protection attempt to fail, which may cause a file creation attempt to fail.</summary>
    ERROR_NO_GUID_TRANSLATION = 560,
    /// <summary>Indicates that an attempt was made to grow an LDT by setting its size, or that the size was not an even number of selectors.</summary>
    ERROR_INVALID_LDT_SIZE = 561,
    /// <summary>Indicates that the starting value for the LDT information was not an integral multiple of the selector size.</summary>
    ERROR_INVALID_LDT_OFFSET = 563,
    /// <summary>Indicates that the user supplied an invalid descriptor when trying to set up Ldt descriptors.</summary>
    ERROR_INVALID_LDT_DESCRIPTOR = 564,
    /// <summary>Indicates a process has too many threads to perform the requested action. For example, assignment of a primary token may only be performed when a process has zero or one threads.</summary>
    ERROR_TOO_MANY_THREADS = 565,
    /// <summary>An attempt was made to operate on a thread within a specific process, but the thread specified is not in the process specified.</summary>
    ERROR_THREAD_NOT_IN_PROCESS = 566,
    /// <summary>Page file quota was exceeded.</summary>
    ERROR_PAGEFILE_QUOTA_EXCEEDED = 567,
    /// <summary>The Netlogon service cannot start because another Netlogon service running in the domain conflicts with the specified role.</summary>
    ERROR_LOGON_SERVER_CONFLICT = 568,
    /// <summary>The SAM database on a Windows Server is significantly out of synchronization with the copy on the Domain Controller. A complete synchronization is required.</summary>
    ERROR_SYNCHRONIZATION_REQUIRED = 569,
    /// <summary>The NtCreateFile API failed. This error should never be returned to an application, it is a place holder for the Windows Lan Manager Redirector to use in its internal error mapping routines.</summary>
    ERROR_NET_OPEN_FAILED = 570,
    /// <summary>{Privilege Failed} The I/O permissions for the process could not be changed.</summary>
    ERROR_IO_PRIVILEGE_FAILED = 571,
    /// <summary>{Application Exit by CTRL+C} The application terminated as a result of a CTRL+C.</summary>
    ERROR_CONTROL_C_EXIT = 572,    // winnt
    /// <summary>{Missing System File} The required system file %hs is bad or missing.</summary>
    ERROR_MISSING_SYSTEMFILE = 573,
    /// <summary>{Application Error} The exception %s (0x%08lx) occurred in the application at location 0x%08lx.</summary>
    ERROR_UNHANDLED_EXCEPTION = 574,
    /// <summary>{Application Error} The application was unable to start correctly (0x%lx). Click OK to close the application.</summary>
    ERROR_APP_INIT_FAILURE = 575,
    /// <summary>{Unable to Create Paging File} The creation of the paging file %hs failed (%lx). The requested size was %ld.</summary>
    ERROR_PAGEFILE_CREATE_FAILED = 576,
    /// <summary>Windows cannot verify the digital signature for this file. A recent hardware or software change might have installed a file that is signed incorrectly or damaged, or that might be malicious software from an unknown source.</summary>
    ERROR_INVALID_IMAGE_HASH = 577,
    /// <summary>{No Paging File Specified} No paging file was specified in the system configuration.</summary>
    ERROR_NO_PAGEFILE = 578,
    /// <summary>{EXCEPTION} A real-mode application issued a floating-point instruction and floating-point hardware is not present.</summary>
    ERROR_ILLEGAL_FLOAT_CONTEXT = 579,
    /// <summary>An event pair synchronization operation was performed using the thread specific client/server event pair object, but no event pair object was associated with the thread.</summary>
    ERROR_NO_EVENT_PAIR = 580,
    /// <summary>A Windows Server has an incorrect configuration.</summary>
    ERROR_DOMAIN_CTRLR_CONFIG_ERROR = 581,
    /// <summary>An illegal character was encountered. For a multi-byte character set this includes a lead byte without a succeeding trail byte. For the Unicode character set this includes the characters 0xFFFF and 0xFFFE.</summary>
    ERROR_ILLEGAL_CHARACTER = 582,
    /// <summary>The Unicode character is not defined in the Unicode character set installed on the system.</summary>
    ERROR_UNDEFINED_CHARACTER = 583,
    /// <summary>The paging file cannot be created on a floppy diskette.</summary>
    ERROR_FLOPPY_VOLUME = 584,
    /// <summary>The system BIOS failed to connect a system interrupt to the device or bus for which the device is connected.</summary>
    ERROR_BIOS_FAILED_TO_CONNECT_INTERRUPT = 585,
    /// <summary>This operation is only allowed for the Primary Domain Controller of the domain.</summary>
    ERROR_BACKUP_CONTROLLER = 586,
    /// <summary>An attempt was made to acquire a mutant such that its maximum count would have been exceeded.</summary>
    ERROR_MUTANT_LIMIT_EXCEEDED = 587,
    /// <summary>A volume has been accessed for which a file system driver is required that has not yet been loaded.</summary>
    ERROR_FS_DRIVER_REQUIRED = 588,
    /// <summary>{Registry File Failure} The registry cannot load the hive (file): %hs or its log or alternate. It is corrupt, absent, or not writable.</summary>
    ERROR_CANNOT_LOAD_REGISTRY_FILE = 589,
    /// <summary>{Unexpected Failure in DebugActiveProcess} An unexpected failure occurred while processing a DebugActiveProcess API request. You may choose OK to terminate the process, or Cancel to ignore the error.</summary>
    ERROR_DEBUG_ATTACH_FAILED = 590,
    /// <summary>{Fatal System Error} The %hs system process terminated unexpectedly with a status of 0x%08x (0x%08x 0x%08x). The system has been shut down.</summary>
    ERROR_SYSTEM_PROCESS_TERMINATED = 591,
    /// <summary>{Data Not Accepted} The TDI client could not handle the data received during an indication.</summary>
    ERROR_DATA_NOT_ACCEPTED = 592,
    /// <summary>NTVDM encountered a hard error.</summary>
    ERROR_VDM_HARD_ERROR = 593,
    /// <summary>{Cancel Timeout} The driver %hs failed to complete a cancelled I/O request in the allotted time.</summary>
    ERROR_DRIVER_CANCEL_TIMEOUT = 594,
    /// <summary>{Reply Message Mismatch} An attempt was made to reply to an LPC message, but the thread specified by the client ID in the message was not waiting on that message.</summary>
    ERROR_REPLY_MESSAGE_MISMATCH = 595,
    /// <summary>{Delayed Write Failed} Windows was unable to save all the data for the file %hs. The data has been lost. This error may be caused by a failure of your computer hardware or network connection. Please try to save this file elsewhere.</summary>
    ERROR_LOST_WRITEBEHIND_DATA = 596,
    /// <summary>The parameter(s) passed to the server in the client/server shared memory window were invalid. Too much data may have been put in the shared memory window.</summary>
    ERROR_CLIENT_SERVER_PARAMETERS_INVALID = 597,
    /// <summary>The stream is not a tiny stream.</summary>
    ERROR_NOT_TINY_STREAM = 598,
    /// <summary>The request must be handled by the stack overflow code.</summary>
    ERROR_STACK_OVERFLOW_READ = 599,
    /// <summary>Internal OFS status codes indicating how an allocation operation is handled. Either it is retried after the containing onode is moved or the extent stream is converted to a large stream.</summary>
    ERROR_CONVERT_TO_LARGE = 600,
    /// <summary>The attempt to find the object found an object matching by ID on the volume but it is out of the scope of the handle used for the operation.</summary>
    ERROR_FOUND_OUT_OF_SCOPE = 601,
    /// <summary>The bucket array must be grown. Retry transaction after doing so.</summary>
    ERROR_ALLOCATE_BUCKET = 602,
    /// <summary>The user/kernel marshalling buffer has overflowed.</summary>
    ERROR_MARSHALL_OVERFLOW = 603,
    /// <summary>The supplied variant structure contains invalid data.</summary>
    ERROR_INVALID_VARIANT = 604,
    /// <summary>The specified buffer contains ill-formed data.</summary>
    ERROR_BAD_COMPRESSION_BUFFER = 605,
    /// <summary>{Audit Failed} An attempt to generate a security audit failed.</summary>
    ERROR_AUDIT_FAILED = 606,
    /// <summary>The timer resolution was not previously set by the current process.</summary>
    ERROR_TIMER_RESOLUTION_NOT_SET = 607,
    /// <summary>There is insufficient account information to log you on.</summary>
    ERROR_INSUFFICIENT_LOGON_INFO = 608,
    /// <summary>{Invalid DLL Entrypoint} The dynamic link library %hs is not written correctly. The stack pointer has been left in an inconsistent state. The entrypoint should be declared as WINAPI or STDCALL. Select YES to fail the DLL load. Select NO to continue execution. Selecting NO may cause the application to operate incorrectly.</summary>
    ERROR_BAD_DLL_ENTRYPOINT = 609,
    /// <summary>{Invalid Service Callback Entrypoint} The %hs service is not written correctly. The stack pointer has been left in an inconsistent state. The callback entrypoint should be declared as WINAPI or STDCALL. Selecting OK will cause the service to continue operation. However, the service process may operate incorrectly.</summary>
    ERROR_BAD_SERVICE_ENTRYPOINT = 610,
    /// <summary>There is an IP address conflict with another system on the network</summary>
    ERROR_IP_ADDRESS_CONFLICT1 = 611,
    /// <summary>There is an IP address conflict with another system on the network</summary>
    ERROR_IP_ADDRESS_CONFLICT2 = 612,
    /// <summary>{Low On Registry Space} The system has reached the maximum size allowed for the system part of the registry. Additional storage requests will be ignored.</summary>
    ERROR_REGISTRY_QUOTA_LIMIT = 613,
    /// <summary>A callback return system service cannot be executed when no callback is active.</summary>
    ERROR_NO_CALLBACK_ACTIVE = 614,
    /// <summary>The password provided is too short to meet the policy of your user account. Please choose a longer password.</summary>
    ERROR_PWD_TOO_SHORT = 615,
    /// <summary>The policy of your user account does not allow you to change passwords too frequently. This is done to prevent users from changing back to a familiar, but potentially discovered, password. If you feel your password has been compromised then please contact your administrator immediately to have a new one assigned.</summary>
    ERROR_PWD_TOO_RECENT = 616,
    /// <summary>You have attempted to change your password to one that you have used in the past. The policy of your user account does not allow this. Please select a password that you have not previously used.</summary>
    ERROR_PWD_HISTORY_CONFLICT = 617,
    /// <summary>The specified compression format is unsupported.</summary>
    ERROR_UNSUPPORTED_COMPRESSION = 618,
    /// <summary>The specified hardware profile configuration is invalid.</summary>
    ERROR_INVALID_HW_PROFILE = 619,
    /// <summary>The specified Plug and Play registry device path is invalid.</summary>
    ERROR_INVALID_PLUGPLAY_DEVICE_PATH = 620,
    /// <summary>The specified quota list is internally inconsistent with its descriptor.</summary>
    ERROR_QUOTA_LIST_INCONSISTENT = 621,
    /// <summary>{Windows Evaluation Notification} The evaluation period for this installation of Windows has expired. This system will shutdown in 1 hour. To restore access to this installation of Windows, please upgrade this installation using a licensed distribution of this product.</summary>
    ERROR_EVALUATION_EXPIRATION = 622,
    /// <summary>{Illegal System DLL Relocation} The system DLL %hs was relocated in memory. The application will not run properly. The relocation occurred because the DLL %hs occupied an address range reserved for Windows system DLLs. The vendor supplying the DLL should be contacted for a new DLL.</summary>
    ERROR_ILLEGAL_DLL_RELOCATION = 623,
    /// <summary>{DLL Initialization Failed} The application failed to initialize because the window station is shutting down.</summary>
    ERROR_DLL_INIT_FAILED_LOGOFF = 624,
    /// <summary>The validation process needs to continue on to the next step.</summary>
    ERROR_VALIDATE_CONTINUE = 625,
    /// <summary>There are no more matches for the current index enumeration.</summary>
    ERROR_NO_MORE_MATCHES = 626,
    /// <summary>The range could not be added to the range list because of a conflict.</summary>
    ERROR_RANGE_LIST_CONFLICT = 627,
    /// <summary>The server process is running under a SID different than that required by client.</summary>
    ERROR_SERVER_SID_MISMATCH = 628,
    /// <summary>A group marked use for deny only cannot be enabled.</summary>
    ERROR_CANT_ENABLE_DENY_ONLY = 629,
    /// <summary>{EXCEPTION} Multiple floating point faults.</summary>
    ERROR_FLOAT_MULTIPLE_FAULTS = 630,    // winnt
    /// <summary>{EXCEPTION} Multiple floating point traps.</summary>
    ERROR_FLOAT_MULTIPLE_TRAPS = 631,    // winnt
    /// <summary>The requested interface is not supported.</summary>
    ERROR_NOINTERFACE = 632,
    /// <summary>{System Standby Failed} The driver %hs does not support standby mode. Updating this driver may allow the system to go to standby mode.</summary>
    ERROR_DRIVER_FAILED_SLEEP = 633,
    /// <summary>The system file %1 has become corrupt and has been replaced.</summary>
    ERROR_CORRUPT_SYSTEM_FILE = 634,
    /// <summary>{Virtual Memory Minimum Too Low} Your system is low on virtual memory. Windows is increasing the size of your virtual memory paging file. During this process, memory requests for some applications may be denied. For more information, see Help.</summary>
    ERROR_COMMITMENT_MINIMUM = 635,
    /// <summary>A device was removed so enumeration must be restarted.</summary>
    ERROR_PNP_RESTART_ENUMERATION = 636,
    /// <summary>{Fatal System Error} The system image %s is not properly signed. The file has been replaced with the signed file. The system has been shut down.</summary>
    ERROR_SYSTEM_IMAGE_BAD_SIGNATURE = 637,
    /// <summary>Device will not start without a reboot.</summary>
    ERROR_PNP_REBOOT_REQUIRED = 638,
    /// <summary>There is not enough power to complete the requested operation.</summary>
    ERROR_INSUFFICIENT_POWER = 639,
    /// <summary> ERROR_MULTIPLE_FAULT_VIOLATION</summary>
    ERROR_MULTIPLE_FAULT_VIOLATION = 640,
    /// <summary>The system is in the process of shutting down.</summary>
    ERROR_SYSTEM_SHUTDOWN = 641,
    /// <summary>An attempt to remove a processes DebugPort was made, but a port was not already associated with the process.</summary>
    ERROR_PORT_NOT_SET = 642,
    /// <summary>This version of Windows is not compatible with the behavior version of directory forest, domain or domain controller.</summary>
    ERROR_DS_VERSION_CHECK_FAILURE = 643,
    /// <summary>The specified range could not be found in the range list.</summary>
    ERROR_RANGE_NOT_FOUND = 644,
    /// <summary>The driver was not loaded because the system is booting into safe mode.</summary>
    ERROR_NOT_SAFE_MODE_DRIVER = 646,
    /// <summary>The driver was not loaded because it failed it&#39;s initialization call.</summary>
    ERROR_FAILED_DRIVER_ENTRY = 647,
    /// <summary>The &quot;%hs&quot; encountered an error while applying power or reading the device configuration. This may be caused by a failure of your hardware or by a poor connection.</summary>
    ERROR_DEVICE_ENUMERATION_ERROR = 648,
    /// <summary>The create operation failed because the name contained at least one mount point which resolves to a volume to which the specified device object is not attached.</summary>
    ERROR_MOUNT_POINT_NOT_RESOLVED = 649,
    /// <summary>The device object parameter is either not a valid device object or is not attached to the volume specified by the file name.</summary>
    ERROR_INVALID_DEVICE_OBJECT_PARAMETER = 650,
    /// <summary>A Machine Check Error has occurred. Please check the system eventlog for additional information.</summary>
    ERROR_MCA_OCCURED = 651,
    /// <summary>There was error [%2] processing the driver database.</summary>
    ERROR_DRIVER_DATABASE_ERROR = 652,
    /// <summary>System hive size has exceeded its limit.</summary>
    ERROR_SYSTEM_HIVE_TOO_LARGE = 653,
    /// <summary>The driver could not be loaded because a previous version of the driver is still in memory.</summary>
    ERROR_DRIVER_FAILED_PRIOR_UNLOAD = 654,
    /// <summary>{Volume Shadow Copy Service} Please wait while the Volume Shadow Copy Service prepares volume %hs for hibernation.</summary>
    ERROR_VOLSNAP_PREPARE_HIBERNATE = 655,
    /// <summary>The system has failed to hibernate (The error code is %hs). Hibernation will be disabled until the system is restarted.</summary>
    ERROR_HIBERNATION_FAILURE = 656,
    /// <summary>The requested operation could not be completed due to a file system limitation</summary>
    ERROR_FILE_SYSTEM_LIMITATION = 665,
    /// <summary>An assertion failure has occurred.</summary>
    ERROR_ASSERTION_FAILURE = 668,
    /// <summary>An error occurred in the ACPI subsystem.</summary>
    ERROR_ACPI_ERROR = 669,
    /// <summary>WOW Assertion Error.</summary>
    ERROR_WOW_ASSERTION = 670,
    /// <summary>A device is missing in the system BIOS MPS table. This device will not be used. Please contact your system vendor for system BIOS update.</summary>
    ERROR_PNP_BAD_MPS_TABLE = 671,
    /// <summary>A translator failed to translate resources.</summary>
    ERROR_PNP_TRANSLATION_FAILED = 672,
    /// <summary>A IRQ translator failed to translate resources.</summary>
    ERROR_PNP_IRQ_TRANSLATION_FAILED = 673,
    /// <summary>Driver %2 returned invalid ID for a child device (%3).</summary>
    ERROR_PNP_INVALID_ID = 674,
    /// <summary>{Kernel Debugger Awakened} the system debugger was awakened by an interrupt.</summary>
    ERROR_WAKE_SYSTEM_DEBUGGER = 675,
    /// <summary>{Handles Closed} Handles to objects have been automatically closed as a result of the requested operation.</summary>
    ERROR_HANDLES_CLOSED = 676,
    /// <summary>{Too Much Information} The specified access control list (ACL) contained more information than was expected.</summary>
    ERROR_EXTRANEOUS_INFORMATION = 677,
    /// <summary>This warning level status indicates that the transaction state already exists for the registry sub-tree, but that a transaction commit was previously aborted. The commit has NOT been completed, but has not been rolled back either (so it may still be committed if desired).</summary>
    ERROR_RXACT_COMMIT_NECESSARY = 678,
    /// <summary>{Media Changed} The media may have changed.</summary>
    ERROR_MEDIA_CHECK = 679,
    /// <summary>{GUID Substitution} During the translation of a global identifier (GUID) to a Windows security ID (SID), no administratively-defined GUID prefix was found. A substitute prefix was used, which will not compromise system security. However, this may provide a more restrictive access than intended.</summary>
    ERROR_GUID_SUBSTITUTION_MADE = 680,
    /// <summary>The create operation stopped after reaching a symbolic link</summary>
    ERROR_STOPPED_ON_SYMLINK = 681,
    /// <summary>A long jump has been executed.</summary>
    ERROR_LONGJUMP = 682,
    /// <summary>The Plug and Play query operation was not successful.</summary>
    ERROR_PLUGPLAY_QUERY_VETOED = 683,
    /// <summary>A frame consolidation has been executed.</summary>
    ERROR_UNWIND_CONSOLIDATE = 684,
    /// <summary>{Registry Hive Recovered} Registry hive (file): %hs was corrupted and it has been recovered. Some data might have been lost.</summary>
    ERROR_REGISTRY_HIVE_RECOVERED = 685,
    /// <summary>The application is attempting to run executable code from the module %hs. This may be insecure. An alternative, %hs, is available. Should the application use the secure module %hs?</summary>
    ERROR_DLL_MIGHT_BE_INSECURE = 686,
    /// <summary>The application is loading executable code from the module %hs. This is secure, but may be incompatible with previous releases of the operating system. An alternative, %hs, is available. Should the application use the secure module %hs?</summary>
    ERROR_DLL_MIGHT_BE_INCOMPATIBLE = 687,
    /// <summary>Debugger did not handle the exception.</summary>
    ERROR_DBG_EXCEPTION_NOT_HANDLED = 688,    // winnt
    /// <summary>Debugger will reply later.</summary>
    ERROR_DBG_REPLY_LATER = 689,
    /// <summary>Debugger cannot provide handle.</summary>
    ERROR_DBG_UNABLE_TO_PROVIDE_HANDLE = 690,
    /// <summary>Debugger terminated thread.</summary>
    ERROR_DBG_TERMINATE_THREAD = 691,    // winnt
    /// <summary>Debugger terminated process.</summary>
    ERROR_DBG_TERMINATE_PROCESS = 692,    // winnt
    /// <summary>Debugger got control C.</summary>
    ERROR_DBG_CONTROL_C = 693,    // winnt
    /// <summary>Debugger printed exception on control C.</summary>
    ERROR_DBG_PRINTEXCEPTION_C = 694,
    /// <summary>Debugger received RIP exception.</summary>
    ERROR_DBG_RIPEXCEPTION = 695,
    /// <summary>Debugger received control break.</summary>
    ERROR_DBG_CONTROL_BREAK = 696,    // winnt
    /// <summary>Debugger command communication exception.</summary>
    ERROR_DBG_COMMAND_EXCEPTION = 697,    // winnt
    /// <summary>{Object Exists} An attempt was made to create an object and the object name already existed.</summary>
    ERROR_OBJECT_NAME_EXISTS = 698,
    /// <summary>{Thread Suspended} A thread termination occurred while the thread was suspended. The thread was resumed, and termination proceeded.</summary>
    ERROR_THREAD_WAS_SUSPENDED = 699,
    /// <summary>{Image Relocated} An image file could not be mapped at the address specified in the image file. Local fixups must be performed on this image.</summary>
    ERROR_IMAGE_NOT_AT_BASE = 700,
    /// <summary>This informational level status indicates that a specified registry sub-tree transaction state did not yet exist and had to be created.</summary>
    ERROR_RXACT_STATE_CREATED = 701,
    /// <summary>{Segment Load} A virtual DOS machine (VDM) is loading, unloading, or moving an MS-DOS or Win16 program segment image. An exception is raised so a debugger can load, unload or track symbols and breakpoints within these 16-bit segments.</summary>
    ERROR_SEGMENT_NOTIFICATION = 702,    // winnt
    /// <summary>{Invalid Current Directory} The process cannot switch to the startup current directory %hs. Select OK to set current directory to %hs, or select CANCEL to exit.</summary>
    ERROR_BAD_CURRENT_DIRECTORY = 703,
    /// <summary>{Redundant Read} To satisfy a read request, the NT fault-tolerant file system successfully read the requested data from a redundant copy. This was done because the file system encountered a failure on a member of the fault-tolerant volume, but was unable to reassign the failing area of the device.</summary>
    ERROR_FT_READ_RECOVERY_FROM_BACKUP = 704,
    /// <summary>{Redundant Write} To satisfy a write request, the NT fault-tolerant file system successfully wrote a redundant copy of the information. This was done because the file system encountered a failure on a member of the fault-tolerant volume, but was not able to reassign the failing area of the device.</summary>
    ERROR_FT_WRITE_RECOVERY = 705,
    /// <summary>{Machine Type Mismatch} The image file %hs is valid, but is for a machine type other than the current machine. Select OK to continue, or CANCEL to fail the DLL load.</summary>
    ERROR_IMAGE_MACHINE_TYPE_MISMATCH = 706,
    /// <summary>{Partial Data Received} The network transport returned partial data to its client. The remaining data will be sent later.</summary>
    ERROR_RECEIVE_PARTIAL = 707,
    /// <summary>{Expedited Data Received} The network transport returned data to its client that was marked as expedited by the remote system.</summary>
    ERROR_RECEIVE_EXPEDITED = 708,
    /// <summary>{Partial Expedited Data Received} The network transport returned partial data to its client and this data was marked as expedited by the remote system. The remaining data will be sent later.</summary>
    ERROR_RECEIVE_PARTIAL_EXPEDITED = 709,
    /// <summary>{TDI Event Done} The TDI indication has completed successfully.</summary>
    ERROR_EVENT_DONE = 710,
    /// <summary>{TDI Event Pending} The TDI indication has entered the pending state.</summary>
    ERROR_EVENT_PENDING = 711,
    /// <summary>Checking file system on %wZ</summary>
    ERROR_CHECKING_FILE_SYSTEM = 712,
    /// <summary>{Fatal Application Exit} %hs</summary>
    ERROR_FATAL_APP_EXIT = 713,
    /// <summary>The specified registry key is referenced by a predefined handle.</summary>
    ERROR_PREDEFINED_HANDLE = 714,
    /// <summary>{Page Unlocked} The page protection of a locked page was changed to &#39;No Access&#39; and the page was unlocked from memory and from the process.</summary>
    ERROR_WAS_UNLOCKED = 715,
    /// <summary>%hs</summary>
    ERROR_SERVICE_NOTIFICATION = 716,
    /// <summary>{Page Locked} One of the pages to lock was already locked.</summary>
    ERROR_WAS_LOCKED = 717,
    /// <summary>Application popup: %1 : %2</summary>
    ERROR_LOG_HARD_ERROR = 718,
    /// <summary> ERROR_ALREADY_WIN32</summary>
    ERROR_ALREADY_WIN32 = 719,
    /// <summary>{Machine Type Mismatch} The image file %hs is valid, but is for a machine type other than the current machine.</summary>
    ERROR_IMAGE_MACHINE_TYPE_MISMATCH_EXE = 720,
    /// <summary>A yield execution was performed and no thread was available to run.</summary>
    ERROR_NO_YIELD_PERFORMED = 721,
    /// <summary>The resumable flag to a timer API was ignored.</summary>
    ERROR_TIMER_RESUME_IGNORED = 722,
    /// <summary>The arbiter has deferred arbitration of these resources to its parent</summary>
    ERROR_ARBITRATION_UNHANDLED = 723,
    /// <summary>The inserted CardBus device cannot be started because of a configuration error on &quot;%hs&quot;.</summary>
    ERROR_CARDBUS_NOT_SUPPORTED = 724,
    /// <summary>The CPUs in this multiprocessor system are not all the same revision level. To use all processors the operating system restricts itself to the features of the least capable processor in the system. Should problems occur with this system, contact the CPU manufacturer to see if this mix of processors is supported.</summary>
    ERROR_MP_PROCESSOR_MISMATCH = 725,
    /// <summary>The system was put into hibernation.</summary>
    ERROR_HIBERNATED = 726,
    /// <summary>The system was resumed from hibernation.</summary>
    ERROR_RESUME_HIBERNATION = 727,
    /// <summary>Windows has detected that the system firmware (BIOS) was updated [previous firmware date = %2, current firmware date %3].</summary>
    ERROR_FIRMWARE_UPDATED = 728,
    /// <summary>A device driver is leaking locked I/O pages causing system degradation. The system has automatically enabled tracking code in order to try and catch the culprit.</summary>
    ERROR_DRIVERS_LEAKING_LOCKED_PAGES = 729,
    /// <summary>The system has awoken</summary>
    ERROR_WAKE_SYSTEM = 730,
    /// <summary> ERROR_WAIT_1</summary>
    ERROR_WAIT_1 = 731,
    /// <summary> ERROR_WAIT_2</summary>
    ERROR_WAIT_2 = 732,
    /// <summary> ERROR_WAIT_3</summary>
    ERROR_WAIT_3 = 733,
    /// <summary> ERROR_WAIT_63</summary>
    ERROR_WAIT_63 = 734,
    /// <summary> ERROR_ABANDONED_WAIT_0</summary>
    ERROR_ABANDONED_WAIT_0 = 735,    // winnt
    /// <summary> ERROR_ABANDONED_WAIT_63</summary>
    ERROR_ABANDONED_WAIT_63 = 736,
    /// <summary> ERROR_USER_APC</summary>
    ERROR_USER_APC = 737,    // winnt
    /// <summary> ERROR_KERNEL_APC</summary>
    ERROR_KERNEL_APC = 738,
    /// <summary> ERROR_ALERTED</summary>
    ERROR_ALERTED = 739,
    /// <summary>The requested operation requires elevation.</summary>
    ERROR_ELEVATION_REQUIRED = 740,
    /// <summary>A reparse should be performed by the Object Manager since the name of the file resulted in a symbolic link.</summary>
    ERROR_REPARSE = 741,
    /// <summary>An open/create operation completed while an oplock break is underway.</summary>
    ERROR_OPLOCK_BREAK_IN_PROGRESS = 742,
    /// <summary>A new volume has been mounted by a file system.</summary>
    ERROR_VOLUME_MOUNTED = 743,
    /// <summary>This success level status indicates that the transaction state already exists for the registry sub-tree, but that a transaction commit was previously aborted. The commit has now been completed.</summary>
    ERROR_RXACT_COMMITTED = 744,
    /// <summary>This indicates that a notify change request has been completed due to closing the handle which made the notify change request.</summary>
    ERROR_NOTIFY_CLEANUP = 745,
    /// <summary>{Connect Failure on Primary Transport} An attempt was made to connect to the remote server %hs on the primary transport, but the connection failed. The computer WAS able to connect on a secondary transport.</summary>
    ERROR_PRIMARY_TRANSPORT_CONNECT_FAILED = 746,
    /// <summary>Page fault was a transition fault.</summary>
    ERROR_PAGE_FAULT_TRANSITION = 747,
    /// <summary>Page fault was a demand zero fault.</summary>
    ERROR_PAGE_FAULT_DEMAND_ZERO = 748,
    /// <summary>Page fault was a demand zero fault.</summary>
    ERROR_PAGE_FAULT_COPY_ON_WRITE = 749,
    /// <summary>Page fault was a demand zero fault.</summary>
    ERROR_PAGE_FAULT_GUARD_PAGE = 750,
    /// <summary>Page fault was satisfied by reading from a secondary storage device.</summary>
    ERROR_PAGE_FAULT_PAGING_FILE = 751,
    /// <summary>Cached page was locked during operation.</summary>
    ERROR_CACHE_PAGE_LOCKED = 752,
    /// <summary>Crash dump exists in paging file.</summary>
    ERROR_CRASH_DUMP = 753,
    /// <summary>Specified buffer contains all zeros.</summary>
    ERROR_BUFFER_ALL_ZEROS = 754,
    /// <summary>A reparse should be performed by the Object Manager since the name of the file resulted in a symbolic link.</summary>
    ERROR_REPARSE_OBJECT = 755,
    /// <summary>The device has succeeded a query-stop and its resource requirements have changed.</summary>
    ERROR_RESOURCE_REQUIREMENTS_CHANGED = 756,
    /// <summary>The translator has translated these resources into the global space and no further translations should be performed.</summary>
    ERROR_TRANSLATION_COMPLETE = 757,
    /// <summary>A process being terminated has no threads to terminate.</summary>
    ERROR_NOTHING_TO_TERMINATE = 758,
    /// <summary>The specified process is not part of a job.</summary>
    ERROR_PROCESS_NOT_IN_JOB = 759,
    /// <summary>The specified process is part of a job.</summary>
    ERROR_PROCESS_IN_JOB = 760,
    /// <summary>{Volume Shadow Copy Service} The system is now ready for hibernation.</summary>
    ERROR_VOLSNAP_HIBERNATE_READY = 761,
    /// <summary>A file system or file system filter driver has successfully completed an FsFilter operation.</summary>
    ERROR_FSFILTER_OP_COMPLETED_SUCCESSFULLY = 762,
    /// <summary>The specified interrupt vector was already connected.</summary>
    ERROR_INTERRUPT_VECTOR_ALREADY_CONNECTED = 763,
    /// <summary>The specified interrupt vector is still connected.</summary>
    ERROR_INTERRUPT_STILL_CONNECTED = 764,
    /// <summary>An operation is blocked waiting for an oplock.</summary>
    ERROR_WAIT_FOR_OPLOCK = 765,
    /// <summary>Debugger handled exception</summary>
    ERROR_DBG_EXCEPTION_HANDLED = 766,    // winnt
    /// <summary>Debugger continued</summary>
    ERROR_DBG_CONTINUE = 767,    // winnt
    /// <summary>An exception occurred in a user mode callback and the kernel callback frame should be removed.</summary>
    ERROR_CALLBACK_POP_STACK = 768,
    /// <summary>Compression is disabled for this volume.</summary>
    ERROR_COMPRESSION_DISABLED = 769,
    /// <summary>The data provider cannot fetch backwards through a result set.</summary>
    ERROR_CANTFETCHBACKWARDS = 770,
    /// <summary>The data provider cannot scroll backwards through a result set.</summary>
    ERROR_CANTSCROLLBACKWARDS = 771,
    /// <summary>The data provider requires that previously fetched data is released before asking for more data.</summary>
    ERROR_ROWSNOTRELEASED = 772,
    /// <summary>The data provider was not able to interpret the flags set for a column binding in an accessor.</summary>
    ERROR_BAD_ACCESSOR_FLAGS = 773,
    /// <summary>One or more errors occurred while processing the request.</summary>
    ERROR_ERRORS_ENCOUNTERED = 774,
    /// <summary>The implementation is not capable of performing the request.</summary>
    ERROR_NOT_CAPABLE = 775,
    /// <summary>The client of a component requested an operation which is not valid given the state of the component instance.</summary>
    ERROR_REQUEST_OUT_OF_SEQUENCE = 776,
    /// <summary>A version number could not be parsed.</summary>
    ERROR_VERSION_PARSE_ERROR = 777,
    /// <summary>The iterator&#39;s start position is invalid.</summary>
    ERROR_BADSTARTPOSITION = 778,
    /// <summary>The hardware has reported an uncorrectable memory error.</summary>
    ERROR_MEMORY_HARDWARE = 779,
    /// <summary>The attempted operation required self healing to be enabled.</summary>
    ERROR_DISK_REPAIR_DISABLED = 780,
    /// <summary>The Desktop heap encountered an error while allocating session memory. There is more information in the system event log.</summary>
    ERROR_INSUFFICIENT_RESOURCE_FOR_SPECIFIED_SHARED_SECTION_SIZE = 781,
    /// <summary>The system power state is transitioning from %2 to %3.</summary>
    ERROR_SYSTEM_POWERSTATE_TRANSITION = 782,
    /// <summary>The system power state is transitioning from %2 to %3 but could enter %4.</summary>
    ERROR_SYSTEM_POWERSTATE_COMPLEX_TRANSITION = 783,
    /// <summary>A thread is getting dispatched with MCA EXCEPTION because of MCA.</summary>
    ERROR_MCA_EXCEPTION = 784,
    /// <summary>Access to %1 is monitored by policy rule %2.</summary>
    ERROR_ACCESS_AUDIT_BY_POLICY = 785,
    /// <summary>Access to %1 has been restricted by your Administrator by policy rule %2.</summary>
    ERROR_ACCESS_DISABLED_NO_SAFER_UI_BY_POLICY = 786,
    /// <summary>A valid hibernation file has been invalidated and should be abandoned.</summary>
    ERROR_ABANDON_HIBERFILE = 787,
    /// <summary>{Delayed Write Failed} Windows was unable to save all the data for the file %hs; the data has been lost. This error may be caused by network connectivity issues. Please try to save this file elsewhere.</summary>
    ERROR_LOST_WRITEBEHIND_DATA_NETWORK_DISCONNECTED = 788,
    /// <summary>{Delayed Write Failed} Windows was unable to save all the data for the file %hs; the data has been lost. This error was returned by the server on which the file exists. Please try to save this file elsewhere.</summary>
    ERROR_LOST_WRITEBEHIND_DATA_NETWORK_SERVER_ERROR = 789,
    /// <summary>{Delayed Write Failed} Windows was unable to save all the data for the file %hs; the data has been lost. This error may be caused if the device has been removed or the media is write-protected.</summary>
    ERROR_LOST_WRITEBEHIND_DATA_LOCAL_DISK_ERROR = 790,
    /// <summary>The resources required for this device conflict with the MCFG table.</summary>
    ERROR_BAD_MCFG_TABLE = 791,
    /// <summary>The oplock that was associated with this handle is now associated with a different handle.</summary>
    ERROR_OPLOCK_SWITCHED_TO_NEW_HANDLE = 800,
    /// <summary>An oplock of the requested level cannot be granted.  An oplock of a lower level may be available.</summary>
    ERROR_CANNOT_GRANT_REQUESTED_OPLOCK = 801,
    /// <summary>The operation did not complete successfully because it would cause an oplock to be broken. The caller has requested that existing oplocks not be broken.</summary>
    ERROR_CANNOT_BREAK_OPLOCK = 802,
    /// <summary>The handle with which this oplock was associated has been closed.  The oplock is now broken.</summary>
    ERROR_OPLOCK_HANDLE_CLOSED = 803,
    /// <summary>The specified access control entry (ACE) does not contain a condition.</summary>
    ERROR_NO_ACE_CONDITION = 804,
    /// <summary>The specified access control entry (ACE) contains an invalid condition.</summary>
    ERROR_INVALID_ACE_CONDITION = 805,
    /// <summary>Access to the extended attribute was denied.</summary>
    ERROR_EA_ACCESS_DENIED = 994,
    /// <summary>The I/O operation has been aborted because of either a thread exit or an application request.</summary>
    ERROR_OPERATION_ABORTED = 995,
    /// <summary>Overlapped I/O event is not in a signaled state.</summary>
    ERROR_IO_INCOMPLETE = 996,
    /// <summary>Overlapped I/O operation is in progress.</summary>
    ERROR_IO_PENDING = 997,    // dderror
    /// <summary>Invalid access to memory location.</summary>
    ERROR_NOACCESS = 998,
    /// <summary>Error performing inpage operation.</summary>
    ERROR_SWAPERROR = 999,
    /// <summary>Recursion too deep; the stack overflowed.</summary>
    ERROR_STACK_OVERFLOW = 1001,
    /// <summary>The window cannot act on the sent message.</summary>
    ERROR_INVALID_MESSAGE = 1002,
    /// <summary>Cannot complete this function.</summary>
    ERROR_CAN_NOT_COMPLETE = 1003,
    /// <summary>Invalid flags.</summary>
    ERROR_INVALID_FLAGS = 1004,
    /// <summary>The volume does not contain a recognized file system. Please make sure that all required file system drivers are loaded and that the volume is not corrupted.</summary>
    ERROR_UNRECOGNIZED_VOLUME = 1005,
    /// <summary>The volume for a file has been externally altered so that the opened file is no longer valid.</summary>
    ERROR_FILE_INVALID = 1006,
    /// <summary>The requested operation cannot be performed in full-screen mode.</summary>
    ERROR_FULLSCREEN_MODE = 1007,
    /// <summary>An attempt was made to reference a token that does not exist.</summary>
    ERROR_NO_TOKEN = 1008,
    /// <summary>The configuration registry database is corrupt.</summary>
    ERROR_BADDB = 1009,
    /// <summary>The configuration registry key is invalid.</summary>
    ERROR_BADKEY = 1010,
    /// <summary>The configuration registry key could not be opened.</summary>
    ERROR_CANTOPEN = 1011,
    /// <summary>The configuration registry key could not be read.</summary>
    ERROR_CANTREAD = 1012,
    /// <summary>The configuration registry key could not be written.</summary>
    ERROR_CANTWRITE = 1013,
    /// <summary>One of the files in the registry database had to be recovered by use of a log or alternate copy. The recovery was successful.</summary>
    ERROR_REGISTRY_RECOVERED = 1014,
    /// <summary>The registry is corrupted. The structure of one of the files containing registry data is corrupted, or the system&#39;s memory image of the file is corrupted, or the file could not be recovered because the alternate copy or log was absent or corrupted.</summary>
    ERROR_REGISTRY_CORRUPT = 1015,
    /// <summary>An I/O operation initiated by the registry failed unrecoverably. The registry could not read in, or write out, or flush, one of the files that contain the system&#39;s image of the registry.</summary>
    ERROR_REGISTRY_IO_FAILED = 1016,
    /// <summary>The system has attempted to load or restore a file into the registry, but the specified file is not in a registry file format.</summary>
    ERROR_NOT_REGISTRY_FILE = 1017,
    /// <summary>Illegal operation attempted on a registry key that has been marked for deletion.</summary>
    ERROR_KEY_DELETED = 1018,
    /// <summary>System could not allocate the required space in a registry log.</summary>
    ERROR_NO_LOG_SPACE = 1019,
    /// <summary>Cannot create a symbolic link in a registry key that already has subkeys or values.</summary>
    ERROR_KEY_HAS_CHILDREN = 1020,
    /// <summary>Cannot create a stable subkey under a volatile parent key.</summary>
    ERROR_CHILD_MUST_BE_VOLATILE = 1021,
    /// <summary>A notify change request is being completed and the information is not being returned in the caller&#39;s buffer. The caller now needs to enumerate the files to find the changes.</summary>
    ERROR_NOTIFY_ENUM_DIR = 1022,
    /// <summary>A stop control has been sent to a service that other running services are dependent on.</summary>
    ERROR_DEPENDENT_SERVICES_RUNNING = 1051,
    /// <summary>The requested control is not valid for this service.</summary>
    ERROR_INVALID_SERVICE_CONTROL = 1052,
    /// <summary>The service did not respond to the start or control request in a timely fashion.</summary>
    ERROR_SERVICE_REQUEST_TIMEOUT = 1053,
    /// <summary>A thread could not be created for the service.</summary>
    ERROR_SERVICE_NO_THREAD = 1054,
    /// <summary>The service database is locked.</summary>
    ERROR_SERVICE_DATABASE_LOCKED = 1055,
    /// <summary>An instance of the service is already running.</summary>
    ERROR_SERVICE_ALREADY_RUNNING = 1056,
    /// <summary>The account name is invalid or does not exist, or the password is invalid for the account name specified.</summary>
    ERROR_INVALID_SERVICE_ACCOUNT = 1057,
    /// <summary>The service cannot be started, either because it is disabled or because it has no enabled devices associated with it.</summary>
    ERROR_SERVICE_DISABLED = 1058,
    /// <summary>Circular service dependency was specified.</summary>
    ERROR_CIRCULAR_DEPENDENCY = 1059,
    /// <summary>The specified service does not exist as an installed service.</summary>
    ERROR_SERVICE_DOES_NOT_EXIST = 1060,
    /// <summary>The service cannot accept control messages at this time.</summary>
    ERROR_SERVICE_CANNOT_ACCEPT_CTRL = 1061,
    /// <summary>The service has not been started.</summary>
    ERROR_SERVICE_NOT_ACTIVE = 1062,
    /// <summary>The service process could not connect to the service controller.</summary>
    ERROR_FAILED_SERVICE_CONTROLLER_CONNECT = 1063,
    /// <summary>An exception occurred in the service when handling the control request.</summary>
    ERROR_EXCEPTION_IN_SERVICE = 1064,
    /// <summary>The database specified does not exist.</summary>
    ERROR_DATABASE_DOES_NOT_EXIST = 1065,
    /// <summary>The service has returned a service-specific error code.</summary>
    ERROR_SERVICE_SPECIFIC_ERROR = 1066,
    /// <summary>The process terminated unexpectedly.</summary>
    ERROR_PROCESS_ABORTED = 1067,
    /// <summary>The dependency service or group failed to start.</summary>
    ERROR_SERVICE_DEPENDENCY_FAIL = 1068,
    /// <summary>The service did not start due to a logon failure.</summary>
    ERROR_SERVICE_LOGON_FAILED = 1069,
    /// <summary>After starting, the service hung in a start-pending state.</summary>
    ERROR_SERVICE_START_HANG = 1070,
    /// <summary>The specified service database lock is invalid.</summary>
    ERROR_INVALID_SERVICE_LOCK = 1071,
    /// <summary>The specified service has been marked for deletion.</summary>
    ERROR_SERVICE_MARKED_FOR_DELETE = 1072,
    /// <summary>The specified service already exists.</summary>
    ERROR_SERVICE_EXISTS = 1073,
    /// <summary>The system is currently running with the last-known-good configuration.</summary>
    ERROR_ALREADY_RUNNING_LKG = 1074,
    /// <summary>The dependency service does not exist or has been marked for deletion.</summary>
    ERROR_SERVICE_DEPENDENCY_DELETED = 1075,
    /// <summary>The current boot has already been accepted for use as the last-known-good control set.</summary>
    ERROR_BOOT_ALREADY_ACCEPTED = 1076,
    /// <summary>No attempts to start the service have been made since the last boot.</summary>
    ERROR_SERVICE_NEVER_STARTED = 1077,
    /// <summary>The name is already in use as either a service name or a service display name.</summary>
    ERROR_DUPLICATE_SERVICE_NAME = 1078,
    /// <summary>The account specified for this service is different from the account specified for other services running in the same process.</summary>
    ERROR_DIFFERENT_SERVICE_ACCOUNT = 1079,
    /// <summary>Failure actions can only be set for Win32 services, not for drivers.</summary>
    ERROR_CANNOT_DETECT_DRIVER_FAILURE = 1080,
    /// <summary>This service runs in the same process as the service control manager. Therefore, the service control manager cannot take action if this service&#39;s process terminates unexpectedly.</summary>
    ERROR_CANNOT_DETECT_PROCESS_ABORT = 1081,
    /// <summary>No recovery program has been configured for this service.</summary>
    ERROR_NO_RECOVERY_PROGRAM = 1082,
    /// <summary>The executable program that this service is configured to run in does not implement the service.</summary>
    ERROR_SERVICE_NOT_IN_EXE = 1083,
    /// <summary>This service cannot be started in Safe Mode</summary>
    ERROR_NOT_SAFEBOOT_SERVICE = 1084,
    /// <summary>The physical end of the tape has been reached.</summary>
    ERROR_END_OF_MEDIA = 1100,
    /// <summary>A tape access reached a filemark.</summary>
    ERROR_FILEMARK_DETECTED = 1101,
    /// <summary>The beginning of the tape or a partition was encountered.</summary>
    ERROR_BEGINNING_OF_MEDIA = 1102,
    /// <summary>A tape access reached the end of a set of files.</summary>
    ERROR_SETMARK_DETECTED = 1103,
    /// <summary>No more data is on the tape.</summary>
    ERROR_NO_DATA_DETECTED = 1104,
    /// <summary>Tape could not be partitioned.</summary>
    ERROR_PARTITION_FAILURE = 1105,
    /// <summary>When accessing a new tape of a multivolume partition, the current block size is incorrect.</summary>
    ERROR_INVALID_BLOCK_LENGTH = 1106,
    /// <summary>Tape partition information could not be found when loading a tape.</summary>
    ERROR_DEVICE_NOT_PARTITIONED = 1107,
    /// <summary>Unable to lock the media eject mechanism.</summary>
    ERROR_UNABLE_TO_LOCK_MEDIA = 1108,
    /// <summary>Unable to unload the media.</summary>
    ERROR_UNABLE_TO_UNLOAD_MEDIA = 1109,
    /// <summary>The media in the drive may have changed.</summary>
    ERROR_MEDIA_CHANGED = 1110,
    /// <summary>The I/O bus was reset.</summary>
    ERROR_BUS_RESET = 1111,
    /// <summary>No media in drive.</summary>
    ERROR_NO_MEDIA_IN_DRIVE = 1112,
    /// <summary>No mapping for the Unicode character exists in the target multi-byte code page.</summary>
    ERROR_NO_UNICODE_TRANSLATION = 1113,
    /// <summary>A dynamic link library (DLL) initialization routine failed.</summary>
    ERROR_DLL_INIT_FAILED = 1114,
    /// <summary>A system shutdown is in progress.</summary>
    ERROR_SHUTDOWN_IN_PROGRESS = 1115,
    /// <summary>Unable to abort the system shutdown because no shutdown was in progress.</summary>
    ERROR_NO_SHUTDOWN_IN_PROGRESS = 1116,
    /// <summary>The request could not be performed because of an I/O device error.</summary>
    ERROR_IO_DEVICE = 1117,
    /// <summary>No serial device was successfully initialized. The serial driver will unload.</summary>
    ERROR_SERIAL_NO_DEVICE = 1118,
    /// <summary>Unable to open a device that was sharing an interrupt request (IRQ) with other devices. At least one other device that uses that IRQ was already opened.</summary>
    ERROR_IRQ_BUSY = 1119,
    /// <summary>A serial I/O operation was completed by another write to the serial port. (The IOCTL_SERIAL_XOFF_COUNTER reached zero.)</summary>
    ERROR_MORE_WRITES = 1120,
    /// <summary>A serial I/O operation completed because the timeout period expired. (The IOCTL_SERIAL_XOFF_COUNTER did not reach zero.)</summary>
    ERROR_COUNTER_TIMEOUT = 1121,
    /// <summary>No ID address mark was found on the floppy disk.</summary>
    ERROR_FLOPPY_ID_MARK_NOT_FOUND = 1122,
    /// <summary>Mismatch between the floppy disk sector ID field and the floppy disk controller track address.</summary>
    ERROR_FLOPPY_WRONG_CYLINDER = 1123,
    /// <summary>The floppy disk controller reported an error that is not recognized by the floppy disk driver.</summary>
    ERROR_FLOPPY_UNKNOWN_ERROR = 1124,
    /// <summary>The floppy disk controller returned inconsistent results in its registers.</summary>
    ERROR_FLOPPY_BAD_REGISTERS = 1125,
    /// <summary>While accessing the hard disk, a recalibrate operation failed, even after retries.</summary>
    ERROR_DISK_RECALIBRATE_FAILED = 1126,
    /// <summary>While accessing the hard disk, a disk operation failed even after retries.</summary>
    ERROR_DISK_OPERATION_FAILED = 1127,
    /// <summary>While accessing the hard disk, a disk controller reset was needed, but even that failed.</summary>
    ERROR_DISK_RESET_FAILED = 1128,
    /// <summary>Physical end of tape encountered.</summary>
    ERROR_EOM_OVERFLOW = 1129,
    /// <summary>Not enough server storage is available to process this command.</summary>
    ERROR_NOT_ENOUGH_SERVER_MEMORY = 1130,
    /// <summary>A potential deadlock condition has been detected.</summary>
    ERROR_POSSIBLE_DEADLOCK = 1131,
    /// <summary>The base address or the file offset specified does not have the proper alignment.</summary>
    ERROR_MAPPED_ALIGNMENT = 1132,
    /// <summary>An attempt to change the system power state was vetoed by another application or driver.</summary>
    ERROR_SET_POWER_STATE_VETOED = 1140,
    /// <summary>The system BIOS failed an attempt to change the system power state.</summary>
    ERROR_SET_POWER_STATE_FAILED = 1141,
    /// <summary>An attempt was made to create more links on a file than the file system supports.</summary>
    ERROR_TOO_MANY_LINKS = 1142,
    /// <summary>The specified program requires a newer version of Windows.</summary>
    ERROR_OLD_WIN_VERSION = 1150,
    /// <summary>The specified program is not a Windows or MS-DOS program.</summary>
    ERROR_APP_WRONG_OS = 1151,
    /// <summary>Cannot start more than one instance of the specified program.</summary>
    ERROR_SINGLE_INSTANCE_APP = 1152,
    /// <summary>The specified program was written for an earlier version of Windows.</summary>
    ERROR_RMODE_APP = 1153,
    /// <summary>One of the library files needed to run this application is damaged.</summary>
    ERROR_INVALID_DLL = 1154,
    /// <summary>No application is associated with the specified file for this operation.</summary>
    ERROR_NO_ASSOCIATION = 1155,
    /// <summary>An error occurred in sending the command to the application.</summary>
    ERROR_DDE_FAIL = 1156,
    /// <summary>One of the library files needed to run this application cannot be found.</summary>
    ERROR_DLL_NOT_FOUND = 1157,
    /// <summary>The current process has used all of its system allowance of handles for Window Manager objects.</summary>
    ERROR_NO_MORE_USER_HANDLES = 1158,
    /// <summary>The message can be used only with synchronous operations.</summary>
    ERROR_MESSAGE_SYNC_ONLY = 1159,
    /// <summary>The indicated source element has no media.</summary>
    ERROR_SOURCE_ELEMENT_EMPTY = 1160,
    /// <summary>The indicated destination element already contains media.</summary>
    ERROR_DESTINATION_ELEMENT_FULL = 1161,
    /// <summary>The indicated element does not exist.</summary>
    ERROR_ILLEGAL_ELEMENT_ADDRESS = 1162,
    /// <summary>The indicated element is part of a magazine that is not present.</summary>
    ERROR_MAGAZINE_NOT_PRESENT = 1163,
    /// <summary>The indicated device requires reinitialization due to hardware errors.</summary>
    ERROR_DEVICE_REINITIALIZATION_NEEDED = 1164,    // dderror
    /// <summary>The device has indicated that cleaning is required before further operations are attempted.</summary>
    ERROR_DEVICE_REQUIRES_CLEANING = 1165,
    /// <summary>The device has indicated that its door is open.</summary>
    ERROR_DEVICE_DOOR_OPEN = 1166,
    /// <summary>The device is not connected.</summary>
    ERROR_DEVICE_NOT_CONNECTED = 1167,
    /// <summary>Element not found.</summary>
    ERROR_NOT_FOUND = 1168,
    /// <summary>There was no match for the specified key in the index.</summary>
    ERROR_NO_MATCH = 1169,
    /// <summary>The property set specified does not exist on the object.</summary>
    ERROR_SET_NOT_FOUND = 1170,
    /// <summary>The point passed to GetMouseMovePoints is not in the buffer.</summary>
    ERROR_POINT_NOT_FOUND = 1171,
    /// <summary>The tracking (workstation) service is not running.</summary>
    ERROR_NO_TRACKING_SERVICE = 1172,
    /// <summary>The Volume ID could not be found.</summary>
    ERROR_NO_VOLUME_ID = 1173,
    /// <summary>Unable to remove the file to be replaced.</summary>
    ERROR_UNABLE_TO_REMOVE_REPLACED = 1175,
    /// <summary>Unable to move the replacement file to the file to be replaced. The file to be replaced has retained its original name.</summary>
    ERROR_UNABLE_TO_MOVE_REPLACEMENT = 1176,
    /// <summary>Unable to move the replacement file to the file to be replaced. The file to be replaced has been renamed using the backup name.</summary>
    ERROR_UNABLE_TO_MOVE_REPLACEMENT_2 = 1177,
    /// <summary>The volume change journal is being deleted.</summary>
    ERROR_JOURNAL_DELETE_IN_PROGRESS = 1178,
    /// <summary>The volume change journal is not active.</summary>
    ERROR_JOURNAL_NOT_ACTIVE = 1179,
    /// <summary>A file was found, but it may not be the correct file.</summary>
    ERROR_POTENTIAL_FILE_FOUND = 1180,
    /// <summary>The journal entry has been deleted from the journal.</summary>
    ERROR_JOURNAL_ENTRY_DELETED = 1181,
    /// <summary>A system shutdown has already been scheduled.</summary>
    ERROR_SHUTDOWN_IS_SCHEDULED = 1190,
    /// <summary>The system shutdown cannot be initiated because there are other users logged on to the computer.</summary>
    ERROR_SHUTDOWN_USERS_LOGGED_ON = 1191,
    /// <summary>The specified device name is invalid.</summary>
    ERROR_BAD_DEVICE = 1200,
    /// <summary>The device is not currently connected but it is a remembered connection.</summary>
    ERROR_CONNECTION_UNAVAIL = 1201,
    /// <summary>The local device name has a remembered connection to another network resource.</summary>
    ERROR_DEVICE_ALREADY_REMEMBERED = 1202,
    /// <summary>The network path was either typed incorrectly, does not exist, or the network provider is not currently available. Please try retyping the path or contact your network administrator.</summary>
    ERROR_NO_NET_OR_BAD_PATH = 1203,
    /// <summary>The specified network provider name is invalid.</summary>
    ERROR_BAD_PROVIDER = 1204,
    /// <summary>Unable to open the network connection profile.</summary>
    ERROR_CANNOT_OPEN_PROFILE = 1205,
    /// <summary>The network connection profile is corrupted.</summary>
    ERROR_BAD_PROFILE = 1206,
    /// <summary>Cannot enumerate a noncontainer.</summary>
    ERROR_NOT_CONTAINER = 1207,
    /// <summary>An extended error has occurred.</summary>
    ERROR_EXTENDED_ERROR = 1208,
    /// <summary>The format of the specified group name is invalid.</summary>
    ERROR_INVALID_GROUPNAME = 1209,
    /// <summary>The format of the specified computer name is invalid.</summary>
    ERROR_INVALID_COMPUTERNAME = 1210,
    /// <summary>The format of the specified event name is invalid.</summary>
    ERROR_INVALID_EVENTNAME = 1211,
    /// <summary>The format of the specified domain name is invalid.</summary>
    ERROR_INVALID_DOMAINNAME = 1212,
    /// <summary>The format of the specified service name is invalid.</summary>
    ERROR_INVALID_SERVICENAME = 1213,
    /// <summary>The format of the specified network name is invalid.</summary>
    ERROR_INVALID_NETNAME = 1214,
    /// <summary>The format of the specified share name is invalid.</summary>
    ERROR_INVALID_SHARENAME = 1215,
    /// <summary>The format of the specified password is invalid.</summary>
    ERROR_INVALID_PASSWORDNAME = 1216,
    /// <summary>The format of the specified message name is invalid.</summary>
    ERROR_INVALID_MESSAGENAME = 1217,
    /// <summary>The format of the specified message destination is invalid.</summary>
    ERROR_INVALID_MESSAGEDEST = 1218,
    /// <summary>Multiple connections to a server or shared resource by the same user, using more than one user name, are not allowed. Disconnect all previous connections to the server or shared resource and try again.</summary>
    ERROR_SESSION_CREDENTIAL_CONFLICT = 1219,
    /// <summary>An attempt was made to establish a session to a network server, but there are already too many sessions established to that server.</summary>
    ERROR_REMOTE_SESSION_LIMIT_EXCEEDED = 1220,
    /// <summary>The workgroup or domain name is already in use by another computer on the network.</summary>
    ERROR_DUP_DOMAINNAME = 1221,
    /// <summary>The network is not present or not started.</summary>
    ERROR_NO_NETWORK = 1222,
    /// <summary>The operation was canceled by the user.</summary>
    ERROR_CANCELLED = 1223,
    /// <summary>The requested operation cannot be performed on a file with a user-mapped section open.</summary>
    ERROR_USER_MAPPED_FILE = 1224,
    /// <summary>The remote computer refused the network connection.</summary>
    ERROR_CONNECTION_REFUSED = 1225,
    /// <summary>The network connection was gracefully closed.</summary>
    ERROR_GRACEFUL_DISCONNECT = 1226,
    /// <summary>The network transport endpoint already has an address associated with it.</summary>
    ERROR_ADDRESS_ALREADY_ASSOCIATED = 1227,
    /// <summary>An address has not yet been associated with the network endpoint.</summary>
    ERROR_ADDRESS_NOT_ASSOCIATED = 1228,
    /// <summary>An operation was attempted on a nonexistent network connection.</summary>
    ERROR_CONNECTION_INVALID = 1229,
    /// <summary>An invalid operation was attempted on an active network connection.</summary>
    ERROR_CONNECTION_ACTIVE = 1230,
    /// <summary>The network location cannot be reached. For information about network troubleshooting, see Windows Help.</summary>
    ERROR_NETWORK_UNREACHABLE = 1231,
    /// <summary>The network location cannot be reached. For information about network troubleshooting, see Windows Help.</summary>
    ERROR_HOST_UNREACHABLE = 1232,
    /// <summary>The network location cannot be reached. For information about network troubleshooting, see Windows Help.</summary>
    ERROR_PROTOCOL_UNREACHABLE = 1233,
    /// <summary>No service is operating at the destination network endpoint on the remote system.</summary>
    ERROR_PORT_UNREACHABLE = 1234,
    /// <summary>The request was aborted.</summary>
    ERROR_REQUEST_ABORTED = 1235,
    /// <summary>The network connection was aborted by the local system.</summary>
    ERROR_CONNECTION_ABORTED = 1236,
    /// <summary>The operation could not be completed. A retry should be performed.</summary>
    ERROR_RETRY = 1237,
    /// <summary>A connection to the server could not be made because the limit on the number of concurrent connections for this account has been reached.</summary>
    ERROR_CONNECTION_COUNT_LIMIT = 1238,
    /// <summary>Attempting to log in during an unauthorized time of day for this account.</summary>
    ERROR_LOGIN_TIME_RESTRICTION = 1239,
    /// <summary>The account is not authorized to log in from this station.</summary>
    ERROR_LOGIN_WKSTA_RESTRICTION = 1240,
    /// <summary>The network address could not be used for the operation requested.</summary>
    ERROR_INCORRECT_ADDRESS = 1241,
    /// <summary>The service is already registered.</summary>
    ERROR_ALREADY_REGISTERED = 1242,
    /// <summary>The specified service does not exist.</summary>
    ERROR_SERVICE_NOT_FOUND = 1243,
    /// <summary>The operation being requested was not performed because the user has not been authenticated.</summary>
    ERROR_NOT_AUTHENTICATED = 1244,
    /// <summary>The operation being requested was not performed because the user has not logged on to the network. The specified service does not exist.</summary>
    ERROR_NOT_LOGGED_ON = 1245,
    /// <summary>Continue with work in progress.</summary>
    ERROR_CONTINUE = 1246,    // dderror
    /// <summary>An attempt was made to perform an initialization operation when initialization has already been completed.</summary>
    ERROR_ALREADY_INITIALIZED = 1247,
    /// <summary>No more local devices.</summary>
    ERROR_NO_MORE_DEVICES = 1248,    // dderror
    /// <summary>The specified site does not exist.</summary>
    ERROR_NO_SUCH_SITE = 1249,
    /// <summary>A domain controller with the specified name already exists.</summary>
    ERROR_DOMAIN_CONTROLLER_EXISTS = 1250,
    /// <summary>This operation is supported only when you are connected to the server.</summary>
    ERROR_ONLY_IF_CONNECTED = 1251,
    /// <summary>The group policy framework should call the extension even if there are no changes.</summary>
    ERROR_OVERRIDE_NOCHANGES = 1252,
    /// <summary>The specified user does not have a valid profile.</summary>
    ERROR_BAD_USER_PROFILE = 1253,
    /// <summary>This operation is not supported on a computer running Windows Server 2003 for Small Business Server</summary>
    ERROR_NOT_SUPPORTED_ON_SBS = 1254,
    /// <summary>The server machine is shutting down.</summary>
    ERROR_SERVER_SHUTDOWN_IN_PROGRESS = 1255,
    /// <summary>The remote system is not available. For information about network troubleshooting, see Windows Help.</summary>
    ERROR_HOST_DOWN = 1256,
    /// <summary>The security identifier provided is not from an account domain.</summary>
    ERROR_NON_ACCOUNT_SID = 1257,
    /// <summary>The security identifier provided does not have a domain component.</summary>
    ERROR_NON_DOMAIN_SID = 1258,
    /// <summary>AppHelp dialog canceled thus preventing the application from starting.</summary>
    ERROR_APPHELP_BLOCK = 1259,
    /// <summary>This program is blocked by group policy. For more information, contact your system administrator.</summary>
    ERROR_ACCESS_DISABLED_BY_POLICY = 1260,
    /// <summary>A program attempt to use an invalid register value. Normally caused by an uninitialized register. This error is Itanium specific.</summary>
    ERROR_REG_NAT_CONSUMPTION = 1261,
    /// <summary>The share is currently offline or does not exist.</summary>
    ERROR_CSCSHARE_OFFLINE = 1262,
    /// <summary>The Kerberos protocol encountered an error while validating the KDC certificate during smartcard logon. There is more information in the system event log.</summary>
    ERROR_PKINIT_FAILURE = 1263,
    /// <summary>The Kerberos protocol encountered an error while attempting to utilize the smartcard subsystem.</summary>
    ERROR_SMARTCARD_SUBSYSTEM_FAILURE = 1264,
    /// <summary>The system detected a possible attempt to compromise security. Please ensure that you can contact the server that authenticated you.</summary>
    ERROR_DOWNGRADE_DETECTED = 1265,
    /// <summary>The machine is locked and cannot be shut down without the force option.</summary>
    ERROR_MACHINE_LOCKED = 1271,
    /// <summary>An application-defined callback gave invalid data when called.</summary>
    ERROR_CALLBACK_SUPPLIED_INVALID_DATA = 1273,
    /// <summary>The group policy framework should call the extension in the synchronous foreground policy refresh.</summary>
    ERROR_SYNC_FOREGROUND_REFRESH_REQUIRED = 1274,
    /// <summary>This driver has been blocked from loading</summary>
    ERROR_DRIVER_BLOCKED = 1275,
    /// <summary>A dynamic link library (DLL) referenced a module that was neither a DLL nor the process&#39;s executable image.</summary>
    ERROR_INVALID_IMPORT_OF_NON_DLL = 1276,
    /// <summary>Windows cannot open this program since it has been disabled.</summary>
    ERROR_ACCESS_DISABLED_WEBBLADE = 1277,
    /// <summary>Windows cannot open this program because the license enforcement system has been tampered with or become corrupted.</summary>
    ERROR_ACCESS_DISABLED_WEBBLADE_TAMPER = 1278,
    /// <summary>A transaction recover failed.</summary>
    ERROR_RECOVERY_FAILURE = 1279,
    /// <summary>The current thread has already been converted to a fiber.</summary>
    ERROR_ALREADY_FIBER = 1280,
    /// <summary>The current thread has already been converted from a fiber.</summary>
    ERROR_ALREADY_THREAD = 1281,
    /// <summary>The system detected an overrun of a stack-based buffer in this application. This overrun could potentially allow a malicious user to gain control of this application.</summary>
    ERROR_STACK_BUFFER_OVERRUN = 1282,
    /// <summary>Data present in one of the parameters is more than the function can operate on.</summary>
    ERROR_PARAMETER_QUOTA_EXCEEDED = 1283,
    /// <summary>An attempt to do an operation on a debug object failed because the object is in the process of being deleted.</summary>
    ERROR_DEBUGGER_INACTIVE = 1284,
    /// <summary>An attempt to delay-load a .dll or get a function address in a delay-loaded .dll failed.</summary>
    ERROR_DELAY_LOAD_FAILED = 1285,
    /// <summary>%1 is a 16-bit application. You do not have permissions to execute 16-bit applications. Check your permissions with your system administrator.</summary>
    ERROR_VDM_DISALLOWED = 1286,
    /// <summary>Insufficient information exists to identify the cause of failure.</summary>
    ERROR_UNIDENTIFIED_ERROR = 1287,
    /// <summary>The parameter passed to a C runtime function is incorrect.</summary>
    ERROR_INVALID_CRUNTIME_PARAMETER = 1288,
    /// <summary>The operation occurred beyond the valid data length of the file.</summary>
    ERROR_BEYOND_VDL = 1289,
    /// <summary>The service start failed since one or more services in the same process have an incompatible service SID type setting. A service with restricted service SID type can only coexist in the same process with other services with a restricted SID type. If the service SID type for this service was just configured, the hosting process must be restarted in order to start this service.</summary>
    ERROR_INCOMPATIBLE_SERVICE_SID_TYPE = 1290,
    /// <summary>The process hosting the driver for this device has been terminated.</summary>
    ERROR_DRIVER_PROCESS_TERMINATED = 1291,
    /// <summary>An operation attempted to exceed an implementation-defined limit.</summary>
    ERROR_IMPLEMENTATION_LIMIT = 1292,
    /// <summary>Either the target process, or the target thread&#39;s containing process, is a protected process.</summary>
    ERROR_PROCESS_IS_PROTECTED = 1293,
    /// <summary>The service notification client is lagging too far behind the current state of services in the machine.</summary>
    ERROR_SERVICE_NOTIFY_CLIENT_LAGGING = 1294,
    /// <summary>The requested file operation failed because the storage quota was exceeded. To free up disk space, move files to a different location or delete unnecessary files. For more information, contact your system administrator.</summary>
    ERROR_DISK_QUOTA_EXCEEDED = 1295,
    /// <summary>The requested file operation failed because the storage policy blocks that type of file. For more information, contact your system administrator.</summary>
    ERROR_CONTENT_BLOCKED = 1296,
    /// <summary>A privilege that the service requires to function properly does not exist in the service account configuration. You may use the Services Microsoft Management Console (MMC) snap-in (services.msc) and the Local Security Settings MMC snap-in (secpol.msc) to view the service configuration and the account configuration.</summary>
    ERROR_INCOMPATIBLE_SERVICE_PRIVILEGE = 1297,
    /// <summary>A thread involved in this operation appears to be unresponsive.</summary>
    ERROR_APP_HANG = 1298,
    /// <summary>Indicates a particular Security ID may not be assigned as the label of an object.</summary>
    ERROR_INVALID_LABEL = 1299,
    /// <summary>Not all privileges or groups referenced are assigned to the caller.</summary>
    ERROR_NOT_ALL_ASSIGNED = 1300,
    /// <summary>Some mapping between account names and security IDs was not done.</summary>
    ERROR_SOME_NOT_MAPPED = 1301,
    /// <summary>No system quota limits are specifically set for this account.</summary>
    ERROR_NO_QUOTAS_FOR_ACCOUNT = 1302,
    /// <summary>No encryption key is available. A well-known encryption key was returned.</summary>
    ERROR_LOCAL_USER_SESSION_KEY = 1303,
    /// <summary>The password is too complex to be converted to a LAN Manager password. The LAN Manager password returned is a NULL string.</summary>
    ERROR_NULL_LM_PASSWORD = 1304,
    /// <summary>The revision level is unknown.</summary>
    ERROR_UNKNOWN_REVISION = 1305,
    /// <summary>Indicates two revision levels are incompatible.</summary>
    ERROR_REVISION_MISMATCH = 1306,
    /// <summary>This security ID may not be assigned as the owner of this object.</summary>
    ERROR_INVALID_OWNER = 1307,
    /// <summary>This security ID may not be assigned as the primary group of an object.</summary>
    ERROR_INVALID_PRIMARY_GROUP = 1308,
    /// <summary>An attempt has been made to operate on an impersonation token by a thread that is not currently impersonating a client.</summary>
    ERROR_NO_IMPERSONATION_TOKEN = 1309,
    /// <summary>The group may not be disabled.</summary>
    ERROR_CANT_DISABLE_MANDATORY = 1310,
    /// <summary>There are currently no logon servers available to service the logon request.</summary>
    ERROR_NO_LOGON_SERVERS = 1311,
    /// <summary>A specified logon session does not exist. It may already have been terminated.</summary>
    ERROR_NO_SUCH_LOGON_SESSION = 1312,
    /// <summary>A specified privilege does not exist.</summary>
    ERROR_NO_SUCH_PRIVILEGE = 1313,
    /// <summary>A required privilege is not held by the client.</summary>
    ERROR_PRIVILEGE_NOT_HELD = 1314,
    /// <summary>The name provided is not a properly formed account name.</summary>
    ERROR_INVALID_ACCOUNT_NAME = 1315,
    /// <summary>The specified account already exists.</summary>
    ERROR_USER_EXISTS = 1316,
    /// <summary>The specified account does not exist.</summary>
    ERROR_NO_SUCH_USER = 1317,
    /// <summary>The specified group already exists.</summary>
    ERROR_GROUP_EXISTS = 1318,
    /// <summary>The specified group does not exist.</summary>
    ERROR_NO_SUCH_GROUP = 1319,
    /// <summary>Either the specified user account is already a member of the specified group, or the specified group cannot be deleted because it contains a member.</summary>
    ERROR_MEMBER_IN_GROUP = 1320,
    /// <summary>The specified user account is not a member of the specified group account.</summary>
    ERROR_MEMBER_NOT_IN_GROUP = 1321,
    /// <summary>The last remaining administration account cannot be disabled or deleted.</summary>
    ERROR_LAST_ADMIN = 1322,
    /// <summary>Unable to update the password. The value provided as the current password is incorrect.</summary>
    ERROR_WRONG_PASSWORD = 1323,
    /// <summary>Unable to update the password. The value provided for the new password contains values that are not allowed in passwords.</summary>
    ERROR_ILL_FORMED_PASSWORD = 1324,
    /// <summary>Unable to update the password. The value provided for the new password does not meet the length, complexity, or history requirements of the domain.</summary>
    ERROR_PASSWORD_RESTRICTION = 1325,
    /// <summary>Logon failure: unknown user name or bad password.</summary>
    ERROR_LOGON_FAILURE = 1326,
    /// <summary>Logon failure: user account restriction. Possible reasons are blank passwords not allowed, logon hour restrictions, or a policy restriction has been enforced.</summary>
    ERROR_ACCOUNT_RESTRICTION = 1327,
    /// <summary>Logon failure: account logon time restriction violation.</summary>
    ERROR_INVALID_LOGON_HOURS = 1328,
    /// <summary>Logon failure: user not allowed to log on to this computer.</summary>
    ERROR_INVALID_WORKSTATION = 1329,
    /// <summary>Logon failure: the specified account password has expired.</summary>
    ERROR_PASSWORD_EXPIRED = 1330,
    /// <summary>Logon failure: account currently disabled.</summary>
    ERROR_ACCOUNT_DISABLED = 1331,
    /// <summary>No mapping between account names and security IDs was done.</summary>
    ERROR_NONE_MAPPED = 1332,
    /// <summary>Too many local user identifiers (LUIDs) were requested at one time.</summary>
    ERROR_TOO_MANY_LUIDS_REQUESTED = 1333,
    /// <summary>No more local user identifiers (LUIDs) are available.</summary>
    ERROR_LUIDS_EXHAUSTED = 1334,
    /// <summary>The subauthority part of a security ID is invalid for this particular use.</summary>
    ERROR_INVALID_SUB_AUTHORITY = 1335,
    /// <summary>The access control list (ACL) structure is invalid.</summary>
    ERROR_INVALID_ACL = 1336,
    /// <summary>The security ID structure is invalid.</summary>
    ERROR_INVALID_SID = 1337,
    /// <summary>The security descriptor structure is invalid.</summary>
    ERROR_INVALID_SECURITY_DESCR = 1338,
    /// <summary>The inherited access control list (ACL) or access control entry (ACE) could not be built.</summary>
    ERROR_BAD_INHERITANCE_ACL = 1340,
    /// <summary>The server is currently disabled.</summary>
    ERROR_SERVER_DISABLED = 1341,
    /// <summary>The server is currently enabled.</summary>
    ERROR_SERVER_NOT_DISABLED = 1342,
    /// <summary>The value provided was an invalid value for an identifier authority.</summary>
    ERROR_INVALID_ID_AUTHORITY = 1343,
    /// <summary>No more memory is available for security information updates.</summary>
    ERROR_ALLOTTED_SPACE_EXCEEDED = 1344,
    /// <summary>The specified attributes are invalid, or incompatible with the attributes for the group as a whole.</summary>
    ERROR_INVALID_GROUP_ATTRIBUTES = 1345,
    /// <summary>Either a required impersonation level was not provided, or the provided impersonation level is invalid.</summary>
    ERROR_BAD_IMPERSONATION_LEVEL = 1346,
    /// <summary>Cannot open an anonymous level security token.</summary>
    ERROR_CANT_OPEN_ANONYMOUS = 1347,
    /// <summary>The validation information class requested was invalid.</summary>
    ERROR_BAD_VALIDATION_CLASS = 1348,
    /// <summary>The type of the token is inappropriate for its attempted use.</summary>
    ERROR_BAD_TOKEN_TYPE = 1349,
    /// <summary>Unable to perform a security operation on an object that has no associated security.</summary>
    ERROR_NO_SECURITY_ON_OBJECT = 1350,
    /// <summary>Configuration information could not be read from the domain controller, either because the machine is unavailable, or access has been denied.</summary>
    ERROR_CANT_ACCESS_DOMAIN_INFO = 1351,
    /// <summary>The security account manager (SAM) or local security authority (LSA) server was in the wrong state to perform the security operation.</summary>
    ERROR_INVALID_SERVER_STATE = 1352,
    /// <summary>The domain was in the wrong state to perform the security operation.</summary>
    ERROR_INVALID_DOMAIN_STATE = 1353,
    /// <summary>This operation is only allowed for the Primary Domain Controller of the domain.</summary>
    ERROR_INVALID_DOMAIN_ROLE = 1354,
    /// <summary>The specified domain either does not exist or could not be contacted.</summary>
    ERROR_NO_SUCH_DOMAIN = 1355,
    /// <summary>The specified domain already exists.</summary>
    ERROR_DOMAIN_EXISTS = 1356,
    /// <summary>An attempt was made to exceed the limit on the number of domains per server.</summary>
    ERROR_DOMAIN_LIMIT_EXCEEDED = 1357,
    /// <summary>Unable to complete the requested operation because of either a catastrophic media failure or a data structure corruption on the disk.</summary>
    ERROR_INTERNAL_DB_CORRUPTION = 1358,
    /// <summary>An internal error occurred.</summary>
    ERROR_INTERNAL_ERROR = 1359,
    /// <summary>Generic access types were contained in an access mask which should already be mapped to nongeneric types.</summary>
    ERROR_GENERIC_NOT_MAPPED = 1360,
    /// <summary>A security descriptor is not in the right format (absolute or self-relative).</summary>
    ERROR_BAD_DESCRIPTOR_FORMAT = 1361,
    /// <summary>The requested action is restricted for use by logon processes only. The calling process has not registered as a logon process.</summary>
    ERROR_NOT_LOGON_PROCESS = 1362,
    /// <summary>Cannot start a new logon session with an ID that is already in use.</summary>
    ERROR_LOGON_SESSION_EXISTS = 1363,
    /// <summary>A specified authentication package is unknown.</summary>
    ERROR_NO_SUCH_PACKAGE = 1364,
    /// <summary>The logon session is not in a state that is consistent with the requested operation.</summary>
    ERROR_BAD_LOGON_SESSION_STATE = 1365,
    /// <summary>The logon session ID is already in use.</summary>
    ERROR_LOGON_SESSION_COLLISION = 1366,
    /// <summary>A logon request contained an invalid logon type value.</summary>
    ERROR_INVALID_LOGON_TYPE = 1367,
    /// <summary>Unable to impersonate using a named pipe until data has been read from that pipe.</summary>
    ERROR_CANNOT_IMPERSONATE = 1368,
    /// <summary>The transaction state of a registry subtree is incompatible with the requested operation.</summary>
    ERROR_RXACT_INVALID_STATE = 1369,
    /// <summary>An internal security database corruption has been encountered.</summary>
    ERROR_RXACT_COMMIT_FAILURE = 1370,
    /// <summary>Cannot perform this operation on built-in accounts.</summary>
    ERROR_SPECIAL_ACCOUNT = 1371,
    /// <summary>Cannot perform this operation on this built-in special group.</summary>
    ERROR_SPECIAL_GROUP = 1372,
    /// <summary>Cannot perform this operation on this built-in special user.</summary>
    ERROR_SPECIAL_USER = 1373,
    /// <summary>The user cannot be removed from a group because the group is currently the user&#39;s primary group.</summary>
    ERROR_MEMBERS_PRIMARY_GROUP = 1374,
    /// <summary>The token is already in use as a primary token.</summary>
    ERROR_TOKEN_ALREADY_IN_USE = 1375,
    /// <summary>The specified local group does not exist.</summary>
    ERROR_NO_SUCH_ALIAS = 1376,
    /// <summary>The specified account name is not a member of the group.</summary>
    ERROR_MEMBER_NOT_IN_ALIAS = 1377,
    /// <summary>The specified account name is already a member of the group.</summary>
    ERROR_MEMBER_IN_ALIAS = 1378,
    /// <summary>The specified local group already exists.</summary>
    ERROR_ALIAS_EXISTS = 1379,
    /// <summary>Logon failure: the user has not been granted the requested logon type at this computer.</summary>
    ERROR_LOGON_NOT_GRANTED = 1380,
    /// <summary>The maximum number of secrets that may be stored in a single system has been exceeded.</summary>
    ERROR_TOO_MANY_SECRETS = 1381,
    /// <summary>The length of a secret exceeds the maximum length allowed.</summary>
    ERROR_SECRET_TOO_LONG = 1382,
    /// <summary>The local security authority database contains an internal inconsistency.</summary>
    ERROR_INTERNAL_DB_ERROR = 1383,
    /// <summary>During a logon attempt, the user&#39;s security context accumulated too many security IDs.</summary>
    ERROR_TOO_MANY_CONTEXT_IDS = 1384,
    /// <summary>Logon failure: the user has not been granted the requested logon type at this computer.</summary>
    ERROR_LOGON_TYPE_NOT_GRANTED = 1385,
    /// <summary>A cross-encrypted password is necessary to change a user password.</summary>
    ERROR_NT_CROSS_ENCRYPTION_REQUIRED = 1386,
    /// <summary>A member could not be added to or removed from the local group because the member does not exist.</summary>
    ERROR_NO_SUCH_MEMBER = 1387,
    /// <summary>A new member could not be added to a local group because the member has the wrong account type.</summary>
    ERROR_INVALID_MEMBER = 1388,
    /// <summary>Too many security IDs have been specified.</summary>
    ERROR_TOO_MANY_SIDS = 1389,
    /// <summary>A cross-encrypted password is necessary to change this user password.</summary>
    ERROR_LM_CROSS_ENCRYPTION_REQUIRED = 1390,
    /// <summary>Indicates an ACL contains no inheritable components.</summary>
    ERROR_NO_INHERITANCE = 1391,
    /// <summary>The file or directory is corrupted and unreadable.</summary>
    ERROR_FILE_CORRUPT = 1392,
    /// <summary>The disk structure is corrupted and unreadable.</summary>
    ERROR_DISK_CORRUPT = 1393,
    /// <summary>There is no user session key for the specified logon session.</summary>
    ERROR_NO_USER_SESSION_KEY = 1394,
    /// <summary>The service being accessed is licensed for a particular number of connections. No more connections can be made to the service at this time because there are already as many connections as the service can accept.</summary>
    ERROR_LICENSE_QUOTA_EXCEEDED = 1395,
    /// <summary>Logon Failure: The target account name is incorrect.</summary>
    ERROR_WRONG_TARGET_NAME = 1396,
    /// <summary>Mutual Authentication failed. The server&#39;s password is out of date at the domain controller.</summary>
    ERROR_MUTUAL_AUTH_FAILED = 1397,
    /// <summary>There is a time and/or date difference between the client and server.</summary>
    ERROR_TIME_SKEW = 1398,
    /// <summary>This operation cannot be performed on the current domain.</summary>
    ERROR_CURRENT_DOMAIN_NOT_ALLOWED = 1399,
    /// <summary>Invalid window handle.</summary>
    ERROR_INVALID_WINDOW_HANDLE = 1400,
    /// <summary>Invalid menu handle.</summary>
    ERROR_INVALID_MENU_HANDLE = 1401,
    /// <summary>Invalid cursor handle.</summary>
    ERROR_INVALID_CURSOR_HANDLE = 1402,
    /// <summary>Invalid accelerator table handle.</summary>
    ERROR_INVALID_ACCEL_HANDLE = 1403,
    /// <summary>Invalid hook handle.</summary>
    ERROR_INVALID_HOOK_HANDLE = 1404,
    /// <summary>Invalid handle to a multiple-window position structure.</summary>
    ERROR_INVALID_DWP_HANDLE = 1405,
    /// <summary>Cannot create a top-level child window.</summary>
    ERROR_TLW_WITH_WSCHILD = 1406,
    /// <summary>Cannot find window class.</summary>
    ERROR_CANNOT_FIND_WND_CLASS = 1407,
    /// <summary>Invalid window; it belongs to other thread.</summary>
    ERROR_WINDOW_OF_OTHER_THREAD = 1408,
    /// <summary>Hot key is already registered.</summary>
    ERROR_HOTKEY_ALREADY_REGISTERED = 1409,
    /// <summary>Class already exists.</summary>
    ERROR_CLASS_ALREADY_EXISTS = 1410,
    /// <summary>Class does not exist.</summary>
    ERROR_CLASS_DOES_NOT_EXIST = 1411,
    /// <summary>Class still has open windows.</summary>
    ERROR_CLASS_HAS_WINDOWS = 1412,
    /// <summary>Invalid index.</summary>
    ERROR_INVALID_INDEX = 1413,
    /// <summary>Invalid icon handle.</summary>
    ERROR_INVALID_ICON_HANDLE = 1414,
    /// <summary>Using private DIALOG window words.</summary>
    ERROR_PRIVATE_DIALOG_INDEX = 1415,
    /// <summary>The list box identifier was not found.</summary>
    ERROR_LISTBOX_ID_NOT_FOUND = 1416,
    /// <summary>No wildcards were found.</summary>
    ERROR_NO_WILDCARD_CHARACTERS = 1417,
    /// <summary>Thread does not have a clipboard open.</summary>
    ERROR_CLIPBOARD_NOT_OPEN = 1418,
    /// <summary>Hot key is not registered.</summary>
    ERROR_HOTKEY_NOT_REGISTERED = 1419,
    /// <summary>The window is not a valid dialog window.</summary>
    ERROR_WINDOW_NOT_DIALOG = 1420,
    /// <summary>Control ID not found.</summary>
    ERROR_CONTROL_ID_NOT_FOUND = 1421,
    /// <summary>Invalid message for a combo box because it does not have an edit control.</summary>
    ERROR_INVALID_COMBOBOX_MESSAGE = 1422,
    /// <summary>The window is not a combo box.</summary>
    ERROR_WINDOW_NOT_COMBOBOX = 1423,
    /// <summary>Height must be less than 256.</summary>
    ERROR_INVALID_EDIT_HEIGHT = 1424,
    /// <summary>Invalid device context (DC) handle.</summary>
    ERROR_DC_NOT_FOUND = 1425,
    /// <summary>Invalid hook procedure type.</summary>
    ERROR_INVALID_HOOK_FILTER = 1426,
    /// <summary>Invalid hook procedure.</summary>
    ERROR_INVALID_FILTER_PROC = 1427,
    /// <summary>Cannot set nonlocal hook without a module handle.</summary>
    ERROR_HOOK_NEEDS_HMOD = 1428,
    /// <summary>This hook procedure can only be set globally.</summary>
    ERROR_GLOBAL_ONLY_HOOK = 1429,
    /// <summary>The journal hook procedure is already installed.</summary>
    ERROR_JOURNAL_HOOK_SET = 1430,
    /// <summary>The hook procedure is not installed.</summary>
    ERROR_HOOK_NOT_INSTALLED = 1431,
    /// <summary>Invalid message for single-selection list box.</summary>
    ERROR_INVALID_LB_MESSAGE = 1432,
    /// <summary>LB_SETCOUNT sent to non-lazy list box.</summary>
    ERROR_SETCOUNT_ON_BAD_LB = 1433,
    /// <summary>This list box does not support tab stops.</summary>
    ERROR_LB_WITHOUT_TABSTOPS = 1434,
    /// <summary>Cannot destroy object created by another thread.</summary>
    ERROR_DESTROY_OBJECT_OF_OTHER_THREAD = 1435,
    /// <summary>Child windows cannot have menus.</summary>
    ERROR_CHILD_WINDOW_MENU = 1436,
    /// <summary>The window does not have a system menu.</summary>
    ERROR_NO_SYSTEM_MENU = 1437,
    /// <summary>Invalid message box style.</summary>
    ERROR_INVALID_MSGBOX_STYLE = 1438,
    /// <summary>Invalid system-wide (SPI_*) parameter.</summary>
    ERROR_INVALID_SPI_VALUE = 1439,
    /// <summary>Screen already locked.</summary>
    ERROR_SCREEN_ALREADY_LOCKED = 1440,
    /// <summary>All handles to windows in a multiple-window position structure must have the same parent.</summary>
    ERROR_HWNDS_HAVE_DIFF_PARENT = 1441,
    /// <summary>The window is not a child window.</summary>
    ERROR_NOT_CHILD_WINDOW = 1442,
    /// <summary>Invalid GW_* command.</summary>
    ERROR_INVALID_GW_COMMAND = 1443,
    /// <summary>Invalid thread identifier.</summary>
    ERROR_INVALID_THREAD_ID = 1444,
    /// <summary>Cannot process a message from a window that is not a multiple document interface (MDI) window.</summary>
    ERROR_NON_MDICHILD_WINDOW = 1445,
    /// <summary>Popup menu already active.</summary>
    ERROR_POPUP_ALREADY_ACTIVE = 1446,
    /// <summary>The window does not have scroll bars.</summary>
    ERROR_NO_SCROLLBARS = 1447,
    /// <summary>Scroll bar range cannot be greater than MAXLONG.</summary>
    ERROR_INVALID_SCROLLBAR_RANGE = 1448,
    /// <summary>Cannot show or remove the window in the way specified.</summary>
    ERROR_INVALID_SHOWWIN_COMMAND = 1449,
    /// <summary>Insufficient system resources exist to complete the requested service.</summary>
    ERROR_NO_SYSTEM_RESOURCES = 1450,
    /// <summary>Insufficient system resources exist to complete the requested service.</summary>
    ERROR_NONPAGED_SYSTEM_RESOURCES = 1451,
    /// <summary>Insufficient system resources exist to complete the requested service.</summary>
    ERROR_PAGED_SYSTEM_RESOURCES = 1452,
    /// <summary>Insufficient quota to complete the requested service.</summary>
    ERROR_WORKING_SET_QUOTA = 1453,
    /// <summary>Insufficient quota to complete the requested service.</summary>
    ERROR_PAGEFILE_QUOTA = 1454,
    /// <summary>The paging file is too small for this operation to complete.</summary>
    ERROR_COMMITMENT_LIMIT = 1455,
    /// <summary>A menu item was not found.</summary>
    ERROR_MENU_ITEM_NOT_FOUND = 1456,
    /// <summary>Invalid keyboard layout handle.</summary>
    ERROR_INVALID_KEYBOARD_HANDLE = 1457,
    /// <summary>Hook type not allowed.</summary>
    ERROR_HOOK_TYPE_NOT_ALLOWED = 1458,
    /// <summary>This operation requires an interactive window station.</summary>
    ERROR_REQUIRES_INTERACTIVE_WINDOWSTATION = 1459,
    /// <summary>This operation returned because the timeout period expired.</summary>
    ERROR_TIMEOUT = 1460,
    /// <summary>Invalid monitor handle.</summary>
    ERROR_INVALID_MONITOR_HANDLE = 1461,
    /// <summary>Incorrect size argument.</summary>
    ERROR_INCORRECT_SIZE = 1462,
    /// <summary>The symbolic link cannot be followed because its type is disabled.</summary>
    ERROR_SYMLINK_CLASS_DISABLED = 1463,
    /// <summary>This application does not support the current operation on symbolic links.</summary>
    ERROR_SYMLINK_NOT_SUPPORTED = 1464,
    /// <summary>Windows was unable to parse the requested XML data.</summary>
    ERROR_XML_PARSE_ERROR = 1465,
    /// <summary>An error was encountered while processing an XML digital signature.</summary>
    ERROR_XMLDSIG_ERROR = 1466,
    /// <summary>This application must be restarted.</summary>
    ERROR_RESTART_APPLICATION = 1467,
    /// <summary>The caller made the connection request in the wrong routing compartment.</summary>
    ERROR_WRONG_COMPARTMENT = 1468,
    /// <summary>There was an AuthIP failure when attempting to connect to the remote host.</summary>
    ERROR_AUTHIP_FAILURE = 1469,
    /// <summary>Insufficient NVRAM resources exist to complete the requested service. A reboot might be required.</summary>
    ERROR_NO_NVRAM_RESOURCES = 1470,
    /// <summary>The event log file is corrupted.</summary>
    ERROR_EVENTLOG_FILE_CORRUPT = 1500,
    /// <summary>No event log file could be opened, so the event logging service did not start.</summary>
    ERROR_EVENTLOG_CANT_START = 1501,
    /// <summary>The event log file is full.</summary>
    ERROR_LOG_FILE_FULL = 1502,
    /// <summary>The event log file has changed between read operations.</summary>
    ERROR_EVENTLOG_FILE_CHANGED = 1503,
    /// <summary>The specified task name is invalid.</summary>
    ERROR_INVALID_TASK_NAME = 1550,
    /// <summary>The specified task index is invalid.</summary>
    ERROR_INVALID_TASK_INDEX = 1551,
    /// <summary>The specified thread is already joining a task.</summary>
    ERROR_THREAD_ALREADY_IN_TASK = 1552,
    /// <summary>The Windows Installer Service could not be accessed. This can occur if the Windows Installer is not correctly installed. Contact your support personnel for assistance.</summary>
    ERROR_INSTALL_SERVICE_FAILURE = 1601,
    /// <summary>User cancelled installation.</summary>
    ERROR_INSTALL_USEREXIT = 1602,
    /// <summary>Fatal error during installation.</summary>
    ERROR_INSTALL_FAILURE = 1603,
    /// <summary>Installation suspended, incomplete.</summary>
    ERROR_INSTALL_SUSPEND = 1604,
    /// <summary>This action is only valid for products that are currently installed.</summary>
    ERROR_UNKNOWN_PRODUCT = 1605,
    /// <summary>Feature ID not registered.</summary>
    ERROR_UNKNOWN_FEATURE = 1606,
    /// <summary>Component ID not registered.</summary>
    ERROR_UNKNOWN_COMPONENT = 1607,
    /// <summary>Unknown property.</summary>
    ERROR_UNKNOWN_PROPERTY = 1608,
    /// <summary>Handle is in an invalid state.</summary>
    ERROR_INVALID_HANDLE_STATE = 1609,
    /// <summary>The configuration data for this product is corrupt. Contact your support personnel.</summary>
    ERROR_BAD_CONFIGURATION = 1610,
    /// <summary>Component qualifier not present.</summary>
    ERROR_INDEX_ABSENT = 1611,
    /// <summary>The installation source for this product is not available. Verify that the source exists and that you can access it.</summary>
    ERROR_INSTALL_SOURCE_ABSENT = 1612,
    /// <summary>This installation package cannot be installed by the Windows Installer service. You must install a Windows service pack that contains a newer version of the Windows Installer service.</summary>
    ERROR_INSTALL_PACKAGE_VERSION = 1613,
    /// <summary>Product is uninstalled.</summary>
    ERROR_PRODUCT_UNINSTALLED = 1614,
    /// <summary>SQL query syntax invalid or unsupported.</summary>
    ERROR_BAD_QUERY_SYNTAX = 1615,
    /// <summary>Record field does not exist.</summary>
    ERROR_INVALID_FIELD = 1616,
    /// <summary>The device has been removed.</summary>
    ERROR_DEVICE_REMOVED = 1617,
    /// <summary>Another installation is already in progress. Complete that installation before proceeding with this install.</summary>
    ERROR_INSTALL_ALREADY_RUNNING = 1618,
    /// <summary>This installation package could not be opened. Verify that the package exists and that you can access it, or contact the application vendor to verify that this is a valid Windows Installer package.</summary>
    ERROR_INSTALL_PACKAGE_OPEN_FAILED = 1619,
    /// <summary>This installation package could not be opened. Contact the application vendor to verify that this is a valid Windows Installer package.</summary>
    ERROR_INSTALL_PACKAGE_INVALID = 1620,
    /// <summary>There was an error starting the Windows Installer service user interface. Contact your support personnel.</summary>
    ERROR_INSTALL_UI_FAILURE = 1621,
    /// <summary>Error opening installation log file. Verify that the specified log file location exists and that you can write to it.</summary>
    ERROR_INSTALL_LOG_FAILURE = 1622,
    /// <summary>The language of this installation package is not supported by your system.</summary>
    ERROR_INSTALL_LANGUAGE_UNSUPPORTED = 1623,
    /// <summary>Error applying transforms. Verify that the specified transform paths are valid.</summary>
    ERROR_INSTALL_TRANSFORM_FAILURE = 1624,
    /// <summary>This installation is forbidden by system policy. Contact your system administrator.</summary>
    ERROR_INSTALL_PACKAGE_REJECTED = 1625,
    /// <summary>Function could not be executed.</summary>
    ERROR_FUNCTION_NOT_CALLED = 1626,
    /// <summary>Function failed during execution.</summary>
    ERROR_FUNCTION_FAILED = 1627,
    /// <summary>Invalid or unknown table specified.</summary>
    ERROR_INVALID_TABLE = 1628,
    /// <summary>Data supplied is of wrong type.</summary>
    ERROR_DATATYPE_MISMATCH = 1629,
    /// <summary>Data of this type is not supported.</summary>
    ERROR_UNSUPPORTED_TYPE = 1630,
    /// <summary>The Windows Installer service failed to start. Contact your support personnel.</summary>
    ERROR_CREATE_FAILED = 1631,
    /// <summary>The Temp folder is on a drive that is full or is inaccessible. Free up space on the drive or verify that you have write permission on the Temp folder.</summary>
    ERROR_INSTALL_TEMP_UNWRITABLE = 1632,
    /// <summary>This installation package is not supported by this processor type. Contact your product vendor.</summary>
    ERROR_INSTALL_PLATFORM_UNSUPPORTED = 1633,
    /// <summary>Component not used on this computer.</summary>
    ERROR_INSTALL_NOTUSED = 1634,
    /// <summary>This update package could not be opened. Verify that the update package exists and that you can access it, or contact the application vendor to verify that this is a valid Windows Installer update package.</summary>
    ERROR_PATCH_PACKAGE_OPEN_FAILED = 1635,
    /// <summary>This update package could not be opened. Contact the application vendor to verify that this is a valid Windows Installer update package.</summary>
    ERROR_PATCH_PACKAGE_INVALID = 1636,
    /// <summary>This update package cannot be processed by the Windows Installer service. You must install a Windows service pack that contains a newer version of the Windows Installer service.</summary>
    ERROR_PATCH_PACKAGE_UNSUPPORTED = 1637,
    /// <summary>Another version of this product is already installed. Installation of this version cannot continue. To configure or remove the existing version of this product, use Add/Remove Programs on the Control Panel.</summary>
    ERROR_PRODUCT_VERSION = 1638,
    /// <summary>Invalid command line argument. Consult the Windows Installer SDK for detailed command line help.</summary>
    ERROR_INVALID_COMMAND_LINE = 1639,
    /// <summary>Only administrators have permission to add, remove, or configure server software during a Terminal services remote session. If you want to install or configure software on the server, contact your network administrator.</summary>
    ERROR_INSTALL_REMOTE_DISALLOWED = 1640,
    /// <summary>The requested operation completed successfully. The system will be restarted so the changes can take effect.</summary>
    ERROR_SUCCESS_REBOOT_INITIATED = 1641,
    /// <summary>The upgrade cannot be installed by the Windows Installer service because the program to be upgraded may be missing, or the upgrade may update a different version of the program. Verify that the program to be upgraded exists on your computer and that you have the correct upgrade.</summary>
    ERROR_PATCH_TARGET_NOT_FOUND = 1642,
    /// <summary>The update package is not permitted by software restriction policy.</summary>
    ERROR_PATCH_PACKAGE_REJECTED = 1643,
    /// <summary>One or more customizations are not permitted by software restriction policy.</summary>
    ERROR_INSTALL_TRANSFORM_REJECTED = 1644,
    /// <summary>The Windows Installer does not permit installation from a Remote Desktop Connection.</summary>
    ERROR_INSTALL_REMOTE_PROHIBITED = 1645,
    /// <summary>Uninstallation of the update package is not supported.</summary>
    ERROR_PATCH_REMOVAL_UNSUPPORTED = 1646,
    /// <summary>The update is not applied to this product.</summary>
    ERROR_UNKNOWN_PATCH = 1647,
    /// <summary>No valid sequence could be found for the set of updates.</summary>
    ERROR_PATCH_NO_SEQUENCE = 1648,
    /// <summary>Update removal was disallowed by policy.</summary>
    ERROR_PATCH_REMOVAL_DISALLOWED = 1649,
    /// <summary>The XML update data is invalid.</summary>
    ERROR_INVALID_PATCH_XML = 1650,
    /// <summary>Windows Installer does not permit updating of managed advertised products. At least one feature of the product must be installed before applying the update.</summary>
    ERROR_PATCH_MANAGED_ADVERTISED_PRODUCT = 1651,
    /// <summary>The Windows Installer service is not accessible in Safe Mode. Please try again when your computer is not in Safe Mode or you can use System Restore to return your machine to a previous good state.</summary>
    ERROR_INSTALL_SERVICE_SAFEBOOT = 1652,
    /// <summary>A fail fast exception occurred. Exception handlers will not be invoked and the process will be terminated immediately.</summary>
    ERROR_FAIL_FAST_EXCEPTION = 1653,
    /// <summary>The string binding is invalid.</summary>
    RPC_S_INVALID_STRING_BINDING = 1700,
    /// <summary>The binding handle is not the correct type.</summary>
    RPC_S_WRONG_KIND_OF_BINDING = 1701,
    /// <summary>The binding handle is invalid.</summary>
    RPC_S_INVALID_BINDING = 1702,
    /// <summary>The RPC protocol sequence is not supported.</summary>
    RPC_S_PROTSEQ_NOT_SUPPORTED = 1703,
    /// <summary>The RPC protocol sequence is invalid.</summary>
    RPC_S_INVALID_RPC_PROTSEQ = 1704,
    /// <summary>The string universal unique identifier (UUID) is invalid.</summary>
    RPC_S_INVALID_STRING_UUID = 1705,
    /// <summary>The endpoint format is invalid.</summary>
    RPC_S_INVALID_ENDPOINT_FORMAT = 1706,
    /// <summary>The network address is invalid.</summary>
    RPC_S_INVALID_NET_ADDR = 1707,
    /// <summary>No endpoint was found.</summary>
    RPC_S_NO_ENDPOINT_FOUND = 1708,
    /// <summary>The timeout value is invalid.</summary>
    RPC_S_INVALID_TIMEOUT = 1709,
    /// <summary>The object universal unique identifier (UUID) was not found.</summary>
    RPC_S_OBJECT_NOT_FOUND = 1710,
    /// <summary>The object universal unique identifier (UUID) has already been registered.</summary>
    RPC_S_ALREADY_REGISTERED = 1711,
    /// <summary>The type universal unique identifier (UUID) has already been registered.</summary>
    RPC_S_TYPE_ALREADY_REGISTERED = 1712,
    /// <summary>The RPC server is already listening.</summary>
    RPC_S_ALREADY_LISTENING = 1713,
    /// <summary>No protocol sequences have been registered.</summary>
    RPC_S_NO_PROTSEQS_REGISTERED = 1714,
    /// <summary>The RPC server is not listening.</summary>
    RPC_S_NOT_LISTENING = 1715,
    /// <summary>The manager type is unknown.</summary>
    RPC_S_UNKNOWN_MGR_TYPE = 1716,
    /// <summary>The interface is unknown.</summary>
    RPC_S_UNKNOWN_IF = 1717,
    /// <summary>There are no bindings.</summary>
    RPC_S_NO_BINDINGS = 1718,
    /// <summary>There are no protocol sequences.</summary>
    RPC_S_NO_PROTSEQS = 1719,
    /// <summary>The endpoint cannot be created.</summary>
    RPC_S_CANT_CREATE_ENDPOINT = 1720,
    /// <summary>Not enough resources are available to complete this operation.</summary>
    RPC_S_OUT_OF_RESOURCES = 1721,
    /// <summary>The RPC server is unavailable.</summary>
    RPC_S_SERVER_UNAVAILABLE = 1722,
    /// <summary>The RPC server is too busy to complete this operation.</summary>
    RPC_S_SERVER_TOO_BUSY = 1723,
    /// <summary>The network options are invalid.</summary>
    RPC_S_INVALID_NETWORK_OPTIONS = 1724,
    /// <summary>There are no remote procedure calls active on this thread.</summary>
    RPC_S_NO_CALL_ACTIVE = 1725,
    /// <summary>The remote procedure call failed.</summary>
    RPC_S_CALL_FAILED = 1726,
    /// <summary>The remote procedure call failed and did not execute.</summary>
    RPC_S_CALL_FAILED_DNE = 1727,
    /// <summary>A remote procedure call (RPC) protocol error occurred.</summary>
    RPC_S_PROTOCOL_ERROR = 1728,
    /// <summary>Access to the HTTP proxy is denied.</summary>
    RPC_S_PROXY_ACCESS_DENIED = 1729,
    /// <summary>The transfer syntax is not supported by the RPC server.</summary>
    RPC_S_UNSUPPORTED_TRANS_SYN = 1730,
    /// <summary>The universal unique identifier (UUID) type is not supported.</summary>
    RPC_S_UNSUPPORTED_TYPE = 1732,
    /// <summary>The tag is invalid.</summary>
    RPC_S_INVALID_TAG = 1733,
    /// <summary>The array bounds are invalid.</summary>
    RPC_S_INVALID_BOUND = 1734,
    /// <summary>The binding does not contain an entry name.</summary>
    RPC_S_NO_ENTRY_NAME = 1735,
    /// <summary>The name syntax is invalid.</summary>
    RPC_S_INVALID_NAME_SYNTAX = 1736,
    /// <summary>The name syntax is not supported.</summary>
    RPC_S_UNSUPPORTED_NAME_SYNTAX = 1737,
    /// <summary>No network address is available to use to construct a universal unique identifier (UUID).</summary>
    RPC_S_UUID_NO_ADDRESS = 1739,
    /// <summary>The endpoint is a duplicate.</summary>
    RPC_S_DUPLICATE_ENDPOINT = 1740,
    /// <summary>The authentication type is unknown.</summary>
    RPC_S_UNKNOWN_AUTHN_TYPE = 1741,
    /// <summary>The maximum number of calls is too small.</summary>
    RPC_S_MAX_CALLS_TOO_SMALL = 1742,
    /// <summary>The string is too long.</summary>
    RPC_S_STRING_TOO_LONG = 1743,
    /// <summary>The RPC protocol sequence was not found.</summary>
    RPC_S_PROTSEQ_NOT_FOUND = 1744,
    /// <summary>The procedure number is out of range.</summary>
    RPC_S_PROCNUM_OUT_OF_RANGE = 1745,
    /// <summary>The binding does not contain any authentication information.</summary>
    RPC_S_BINDING_HAS_NO_AUTH = 1746,
    /// <summary>The authentication service is unknown.</summary>
    RPC_S_UNKNOWN_AUTHN_SERVICE = 1747,
    /// <summary>The authentication level is unknown.</summary>
    RPC_S_UNKNOWN_AUTHN_LEVEL = 1748,
    /// <summary>The security context is invalid.</summary>
    RPC_S_INVALID_AUTH_IDENTITY = 1749,
    /// <summary>The authorization service is unknown.</summary>
    RPC_S_UNKNOWN_AUTHZ_SERVICE = 1750,
    /// <summary>The entry is invalid.</summary>
    EPT_S_INVALID_ENTRY = 1751,
    /// <summary>The server endpoint cannot perform the operation.</summary>
    EPT_S_CANT_PERFORM_OP = 1752,
    /// <summary>There are no more endpoints available from the endpoint mapper.</summary>
    EPT_S_NOT_REGISTERED = 1753,
    /// <summary>No interfaces have been exported.</summary>
    RPC_S_NOTHING_TO_EXPORT = 1754,
    /// <summary>The entry name is incomplete.</summary>
    RPC_S_INCOMPLETE_NAME = 1755,
    /// <summary>The version option is invalid.</summary>
    RPC_S_INVALID_VERS_OPTION = 1756,
    /// <summary>There are no more members.</summary>
    RPC_S_NO_MORE_MEMBERS = 1757,
    /// <summary>There is nothing to unexport.</summary>
    RPC_S_NOT_ALL_OBJS_UNEXPORTED = 1758,
    /// <summary>The interface was not found.</summary>
    RPC_S_INTERFACE_NOT_FOUND = 1759,
    /// <summary>The entry already exists.</summary>
    RPC_S_ENTRY_ALREADY_EXISTS = 1760,
    /// <summary>The entry is not found.</summary>
    RPC_S_ENTRY_NOT_FOUND = 1761,
    /// <summary>The name service is unavailable.</summary>
    RPC_S_NAME_SERVICE_UNAVAILABLE = 1762,
    /// <summary>The network address family is invalid.</summary>
    RPC_S_INVALID_NAF_ID = 1763,
    /// <summary>The requested operation is not supported.</summary>
    RPC_S_CANNOT_SUPPORT = 1764,
    /// <summary>No security context is available to allow impersonation.</summary>
    RPC_S_NO_CONTEXT_AVAILABLE = 1765,
    /// <summary>An internal error occurred in a remote procedure call (RPC).</summary>
    RPC_S_INTERNAL_ERROR = 1766,
    /// <summary>The RPC server attempted an integer division by zero.</summary>
    RPC_S_ZERO_DIVIDE = 1767,
    /// <summary>An addressing error occurred in the RPC server.</summary>
    RPC_S_ADDRESS_ERROR = 1768,
    /// <summary>A floating-point operation at the RPC server caused a division by zero.</summary>
    RPC_S_FP_DIV_ZERO = 1769,
    /// <summary>A floating-point underflow occurred at the RPC server.</summary>
    RPC_S_FP_UNDERFLOW = 1770,
    /// <summary>A floating-point overflow occurred at the RPC server.</summary>
    RPC_S_FP_OVERFLOW = 1771,
    /// <summary>The list of RPC servers available for the binding of auto handles has been exhausted.</summary>
    RPC_X_NO_MORE_ENTRIES = 1772,
    /// <summary>Unable to open the character translation table file.</summary>
    RPC_X_SS_CHAR_TRANS_OPEN_FAIL = 1773,
    /// <summary>The file containing the character translation table has fewer than 512 bytes.</summary>
    RPC_X_SS_CHAR_TRANS_SHORT_FILE = 1774,
    /// <summary>A null context handle was passed from the client to the host during a remote procedure call.</summary>
    RPC_X_SS_IN_NULL_CONTEXT = 1775,
    /// <summary>The context handle changed during a remote procedure call.</summary>
    RPC_X_SS_CONTEXT_DAMAGED = 1777,
    /// <summary>The binding handles passed to a remote procedure call do not match.</summary>
    RPC_X_SS_HANDLES_MISMATCH = 1778,
    /// <summary>The stub is unable to get the remote procedure call handle.</summary>
    RPC_X_SS_CANNOT_GET_CALL_HANDLE = 1779,
    /// <summary>A null reference pointer was passed to the stub.</summary>
    RPC_X_NULL_REF_POINTER = 1780,
    /// <summary>The enumeration value is out of range.</summary>
    RPC_X_ENUM_VALUE_OUT_OF_RANGE = 1781,
    /// <summary>The byte count is too small.</summary>
    RPC_X_BYTE_COUNT_TOO_SMALL = 1782,
    /// <summary>The stub received bad data.</summary>
    RPC_X_BAD_STUB_DATA = 1783,
    /// <summary>The supplied user buffer is not valid for the requested operation.</summary>
    ERROR_INVALID_USER_BUFFER = 1784,
    /// <summary>The disk media is not recognized. It may not be formatted.</summary>
    ERROR_UNRECOGNIZED_MEDIA = 1785,
    /// <summary>The workstation does not have a trust secret.</summary>
    ERROR_NO_TRUST_LSA_SECRET = 1786,
    /// <summary>The security database on the server does not have a computer account for this workstation trust relationship.</summary>
    ERROR_NO_TRUST_SAM_ACCOUNT = 1787,
    /// <summary>The trust relationship between the primary domain and the trusted domain failed.</summary>
    ERROR_TRUSTED_DOMAIN_FAILURE = 1788,
    /// <summary>The trust relationship between this workstation and the primary domain failed.</summary>
    ERROR_TRUSTED_RELATIONSHIP_FAILURE = 1789,
    /// <summary>The network logon failed.</summary>
    ERROR_TRUST_FAILURE = 1790,
    /// <summary>A remote procedure call is already in progress for this thread.</summary>
    RPC_S_CALL_IN_PROGRESS = 1791,
    /// <summary>An attempt was made to logon, but the network logon service was not started.</summary>
    ERROR_NETLOGON_NOT_STARTED = 1792,
    /// <summary>The user&#39;s account has expired.</summary>
    ERROR_ACCOUNT_EXPIRED = 1793,
    /// <summary>The redirector is in use and cannot be unloaded.</summary>
    ERROR_REDIRECTOR_HAS_OPEN_HANDLES = 1794,
    /// <summary>The specified printer driver is already installed.</summary>
    ERROR_PRINTER_DRIVER_ALREADY_INSTALLED = 1795,
    /// <summary>The specified port is unknown.</summary>
    ERROR_UNKNOWN_PORT = 1796,
    /// <summary>The printer driver is unknown.</summary>
    ERROR_UNKNOWN_PRINTER_DRIVER = 1797,
    /// <summary>The print processor is unknown.</summary>
    ERROR_UNKNOWN_PRINTPROCESSOR = 1798,
    /// <summary>The specified separator file is invalid.</summary>
    ERROR_INVALID_SEPARATOR_FILE = 1799,
    /// <summary>The specified priority is invalid.</summary>
    ERROR_INVALID_PRIORITY = 1800,
    /// <summary>The printer name is invalid.</summary>
    ERROR_INVALID_PRINTER_NAME = 1801,
    /// <summary>The printer already exists.</summary>
    ERROR_PRINTER_ALREADY_EXISTS = 1802,
    /// <summary>The printer command is invalid.</summary>
    ERROR_INVALID_PRINTER_COMMAND = 1803,
    /// <summary>The specified datatype is invalid.</summary>
    ERROR_INVALID_DATATYPE = 1804,
    /// <summary>The environment specified is invalid.</summary>
    ERROR_INVALID_ENVIRONMENT = 1805,
    /// <summary>There are no more bindings.</summary>
    RPC_S_NO_MORE_BINDINGS = 1806,
    /// <summary>The account used is an interdomain trust account. Use your global user account or local user account to access this server.</summary>
    ERROR_NOLOGON_INTERDOMAIN_TRUST_ACCOUNT = 1807,
    /// <summary>The account used is a computer account. Use your global user account or local user account to access this server.</summary>
    ERROR_NOLOGON_WORKSTATION_TRUST_ACCOUNT = 1808,
    /// <summary>The account used is a server trust account. Use your global user account or local user account to access this server.</summary>
    ERROR_NOLOGON_SERVER_TRUST_ACCOUNT = 1809,
    /// <summary>The name or security ID (SID) of the domain specified is inconsistent with the trust information for that domain.</summary>
    ERROR_DOMAIN_TRUST_INCONSISTENT = 1810,
    /// <summary>The server is in use and cannot be unloaded.</summary>
    ERROR_SERVER_HAS_OPEN_HANDLES = 1811,
    /// <summary>The specified image file did not contain a resource section.</summary>
    ERROR_RESOURCE_DATA_NOT_FOUND = 1812,
    /// <summary>The specified resource type cannot be found in the image file.</summary>
    ERROR_RESOURCE_TYPE_NOT_FOUND = 1813,
    /// <summary>The specified resource name cannot be found in the image file.</summary>
    ERROR_RESOURCE_NAME_NOT_FOUND = 1814,
    /// <summary>The specified resource language ID cannot be found in the image file.</summary>
    ERROR_RESOURCE_LANG_NOT_FOUND = 1815,
    /// <summary>Not enough quota is available to process this command.</summary>
    ERROR_NOT_ENOUGH_QUOTA = 1816,
    /// <summary>No interfaces have been registered.</summary>
    RPC_S_NO_INTERFACES = 1817,
    /// <summary>The remote procedure call was cancelled.</summary>
    RPC_S_CALL_CANCELLED = 1818,
    /// <summary>The binding handle does not contain all required information.</summary>
    RPC_S_BINDING_INCOMPLETE = 1819,
    /// <summary>A communications failure occurred during a remote procedure call.</summary>
    RPC_S_COMM_FAILURE = 1820,
    /// <summary>The requested authentication level is not supported.</summary>
    RPC_S_UNSUPPORTED_AUTHN_LEVEL = 1821,
    /// <summary>No principal name registered.</summary>
    RPC_S_NO_PRINC_NAME = 1822,
    /// <summary>The error specified is not a valid Windows RPC error code.</summary>
    RPC_S_NOT_RPC_ERROR = 1823,
    /// <summary>A UUID that is valid only on this computer has been allocated.</summary>
    RPC_S_UUID_LOCAL_ONLY = 1824,
    /// <summary>A security package specific error occurred.</summary>
    RPC_S_SEC_PKG_ERROR = 1825,
    /// <summary>Thread is not canceled.</summary>
    RPC_S_NOT_CANCELLED = 1826,
    /// <summary>Invalid operation on the encoding/decoding handle.</summary>
    RPC_X_INVALID_ES_ACTION = 1827,
    /// <summary>Incompatible version of the serializing package.</summary>
    RPC_X_WRONG_ES_VERSION = 1828,
    /// <summary>Incompatible version of the RPC stub.</summary>
    RPC_X_WRONG_STUB_VERSION = 1829,
    /// <summary>The RPC pipe object is invalid or corrupted.</summary>
    RPC_X_INVALID_PIPE_OBJECT = 1830,
    /// <summary>An invalid operation was attempted on an RPC pipe object.</summary>
    RPC_X_WRONG_PIPE_ORDER = 1831,
    /// <summary>Unsupported RPC pipe version.</summary>
    RPC_X_WRONG_PIPE_VERSION = 1832,
    /// <summary>HTTP proxy server rejected the connection because the cookie authentication failed.</summary>
    RPC_S_COOKIE_AUTH_FAILED = 1833,
    /// <summary>The group member was not found.</summary>
    RPC_S_GROUP_MEMBER_NOT_FOUND = 1898,
    /// <summary>The endpoint mapper database entry could not be created.</summary>
    EPT_S_CANT_CREATE = 1899,
    /// <summary>The object universal unique identifier (UUID) is the nil UUID.</summary>
    RPC_S_INVALID_OBJECT = 1900,
    /// <summary>The specified time is invalid.</summary>
    ERROR_INVALID_TIME = 1901,
    /// <summary>The specified form name is invalid.</summary>
    ERROR_INVALID_FORM_NAME = 1902,
    /// <summary>The specified form size is invalid.</summary>
    ERROR_INVALID_FORM_SIZE = 1903,
    /// <summary>The specified printer handle is already being waited on</summary>
    ERROR_ALREADY_WAITING = 1904,
    /// <summary>The specified printer has been deleted.</summary>
    ERROR_PRINTER_DELETED = 1905,
    /// <summary>The state of the printer is invalid.</summary>
    ERROR_INVALID_PRINTER_STATE = 1906,
    /// <summary>The user&#39;s password must be changed before logging on the first time.</summary>
    ERROR_PASSWORD_MUST_CHANGE = 1907,
    /// <summary>Could not find the domain controller for this domain.</summary>
    ERROR_DOMAIN_CONTROLLER_NOT_FOUND = 1908,
    /// <summary>The referenced account is currently locked out and may not be logged on to.</summary>
    ERROR_ACCOUNT_LOCKED_OUT = 1909,
    /// <summary>The object exporter specified was not found.</summary>
    OR_INVALID_OXID = 1910,
    /// <summary>The object specified was not found.</summary>
    OR_INVALID_OID = 1911,
    /// <summary>The object resolver set specified was not found.</summary>
    OR_INVALID_SET = 1912,
    /// <summary>Some data remains to be sent in the request buffer.</summary>
    RPC_S_SEND_INCOMPLETE = 1913,
    /// <summary>Invalid asynchronous remote procedure call handle.</summary>
    RPC_S_INVALID_ASYNC_HANDLE = 1914,
    /// <summary>Invalid asynchronous RPC call handle for this operation.</summary>
    RPC_S_INVALID_ASYNC_CALL = 1915,
    /// <summary>The RPC pipe object has already been closed.</summary>
    RPC_X_PIPE_CLOSED = 1916,
    /// <summary>The RPC call completed before all pipes were processed.</summary>
    RPC_X_PIPE_DISCIPLINE_ERROR = 1917,
    /// <summary>No more data is available from the RPC pipe.</summary>
    RPC_X_PIPE_EMPTY = 1918,
    /// <summary>No site name is available for this machine.</summary>
    ERROR_NO_SITENAME = 1919,
    /// <summary>The file cannot be accessed by the system.</summary>
    ERROR_CANT_ACCESS_FILE = 1920,
    /// <summary>The name of the file cannot be resolved by the system.</summary>
    ERROR_CANT_RESOLVE_FILENAME = 1921,
    /// <summary>The entry is not of the expected type.</summary>
    RPC_S_ENTRY_TYPE_MISMATCH = 1922,
    /// <summary>Not all object UUIDs could be exported to the specified entry.</summary>
    RPC_S_NOT_ALL_OBJS_EXPORTED = 1923,
    /// <summary>Interface could not be exported to the specified entry.</summary>
    RPC_S_INTERFACE_NOT_EXPORTED = 1924,
    /// <summary>The specified profile entry could not be added.</summary>
    RPC_S_PROFILE_NOT_ADDED = 1925,
    /// <summary>The specified profile element could not be added.</summary>
    RPC_S_PRF_ELT_NOT_ADDED = 1926,
    /// <summary>The specified profile element could not be removed.</summary>
    RPC_S_PRF_ELT_NOT_REMOVED = 1927,
    /// <summary>The group element could not be added.</summary>
    RPC_S_GRP_ELT_NOT_ADDED = 1928,
    /// <summary>The group element could not be removed.</summary>
    RPC_S_GRP_ELT_NOT_REMOVED = 1929,
    /// <summary>The printer driver is not compatible with a policy enabled on your computer that blocks NT 4.0 drivers.</summary>
    ERROR_KM_DRIVER_BLOCKED = 1930,
    /// <summary>The context has expired and can no longer be used.</summary>
    ERROR_CONTEXT_EXPIRED = 1931,
    /// <summary>The current user&#39;s delegated trust creation quota has been exceeded.</summary>
    ERROR_PER_USER_TRUST_QUOTA_EXCEEDED = 1932,
    /// <summary>The total delegated trust creation quota has been exceeded.</summary>
    ERROR_ALL_USER_TRUST_QUOTA_EXCEEDED = 1933,
    /// <summary>The current user&#39;s delegated trust deletion quota has been exceeded.</summary>
    ERROR_USER_DELETE_TRUST_QUOTA_EXCEEDED = 1934,
    /// <summary>Logon Failure: The machine you are logging onto is protected by an authentication firewall. The specified account is not allowed to authenticate to the machine.</summary>
    ERROR_AUTHENTICATION_FIREWALL_FAILED = 1935,
    /// <summary>Remote connections to the Print Spooler are blocked by a policy set on your machine.</summary>
    ERROR_REMOTE_PRINT_CONNECTIONS_BLOCKED = 1936,
    /// <summary>Logon Failure: Authentication failed because NTLM authentication has been disabled.</summary>
    ERROR_NTLM_BLOCKED = 1937,
    /// <summary>The pixel format is invalid.</summary>
    ERROR_INVALID_PIXEL_FORMAT = 2000,
    /// <summary>The specified driver is invalid.</summary>
    ERROR_BAD_DRIVER = 2001,
    /// <summary>The window style or class attribute is invalid for this operation.</summary>
    ERROR_INVALID_WINDOW_STYLE = 2002,
    /// <summary>The requested metafile operation is not supported.</summary>
    ERROR_METAFILE_NOT_SUPPORTED = 2003,
    /// <summary>The requested transformation operation is not supported.</summary>
    ERROR_TRANSFORM_NOT_SUPPORTED = 2004,
    /// <summary>The requested clipping operation is not supported.</summary>
    ERROR_CLIPPING_NOT_SUPPORTED = 2005,
    /// <summary>The specified color management module is invalid.</summary>
    ERROR_INVALID_CMM = 2010,
    /// <summary>The specified color profile is invalid.</summary>
    ERROR_INVALID_PROFILE = 2011,
    /// <summary>The specified tag was not found.</summary>
    ERROR_TAG_NOT_FOUND = 2012,
    /// <summary>A required tag is not present.</summary>
    ERROR_TAG_NOT_PRESENT = 2013,
    /// <summary>The specified tag is already present.</summary>
    ERROR_DUPLICATE_TAG = 2014,
    /// <summary>The specified color profile is not associated with the specified device.</summary>
    ERROR_PROFILE_NOT_ASSOCIATED_WITH_DEVICE = 2015,
    /// <summary>The specified color profile was not found.</summary>
    ERROR_PROFILE_NOT_FOUND = 2016,
    /// <summary>The specified color space is invalid.</summary>
    ERROR_INVALID_COLORSPACE = 2017,
    /// <summary>Image Color Management is not enabled.</summary>
    ERROR_ICM_NOT_ENABLED = 2018,
    /// <summary>There was an error while deleting the color transform.</summary>
    ERROR_DELETING_ICM_XFORM = 2019,
    /// <summary>The specified color transform is invalid.</summary>
    ERROR_INVALID_TRANSFORM = 2020,
    /// <summary>The specified transform does not match the bitmap&#39;s color space.</summary>
    ERROR_COLORSPACE_MISMATCH = 2021,
    /// <summary>The specified named color index is not present in the profile.</summary>
    ERROR_INVALID_COLORINDEX = 2022,
    /// <summary>The specified profile is intended for a device of a different type than the specified device.</summary>
    ERROR_PROFILE_DOES_NOT_MATCH_DEVICE = 2023,
    /// <summary>The network connection was made successfully, but the user had to be prompted for a password other than the one originally specified.</summary>
    ERROR_CONNECTED_OTHER_PASSWORD = 2108,
    /// <summary>The network connection was made successfully using default credentials.</summary>
    ERROR_CONNECTED_OTHER_PASSWORD_DEFAULT = 2109,
    /// <summary>The specified username is invalid.</summary>
    ERROR_BAD_USERNAME = 2202,
    /// <summary>This network connection does not exist.</summary>
    ERROR_NOT_CONNECTED = 2250,
    /// <summary>This network connection has files open or requests pending.</summary>
    ERROR_OPEN_FILES = 2401,
    /// <summary>Active connections still exist.</summary>
    ERROR_ACTIVE_CONNECTIONS = 2402,
    /// <summary>The device is in use by an active process and cannot be disconnected.</summary>
    ERROR_DEVICE_IN_USE = 2404,
    /// <summary>The specified print monitor is unknown.</summary>
    ERROR_UNKNOWN_PRINT_MONITOR = 3000,
    /// <summary>The specified printer driver is currently in use.</summary>
    ERROR_PRINTER_DRIVER_IN_USE = 3001,
    /// <summary>The spool file was not found.</summary>
    ERROR_SPOOL_FILE_NOT_FOUND = 3002,
    /// <summary>A StartDocPrinter call was not issued.</summary>
    ERROR_SPL_NO_STARTDOC = 3003,
    /// <summary>An AddJob call was not issued.</summary>
    ERROR_SPL_NO_ADDJOB = 3004,
    /// <summary>The specified print processor has already been installed.</summary>
    ERROR_PRINT_PROCESSOR_ALREADY_INSTALLED = 3005,
    /// <summary>The specified print monitor has already been installed.</summary>
    ERROR_PRINT_MONITOR_ALREADY_INSTALLED = 3006,
    /// <summary>The specified print monitor does not have the required functions.</summary>
    ERROR_INVALID_PRINT_MONITOR = 3007,
    /// <summary>The specified print monitor is currently in use.</summary>
    ERROR_PRINT_MONITOR_IN_USE = 3008,
    /// <summary>The requested operation is not allowed when there are jobs queued to the printer.</summary>
    ERROR_PRINTER_HAS_JOBS_QUEUED = 3009,
    /// <summary>The requested operation is successful. Changes will not be effective until the system is rebooted.</summary>
    ERROR_SUCCESS_REBOOT_REQUIRED = 3010,
    /// <summary>The requested operation is successful. Changes will not be effective until the service is restarted.</summary>
    ERROR_SUCCESS_RESTART_REQUIRED = 3011,
    /// <summary>No printers were found.</summary>
    ERROR_PRINTER_NOT_FOUND = 3012,
    /// <summary>The printer driver is known to be unreliable.</summary>
    ERROR_PRINTER_DRIVER_WARNED = 3013,
    /// <summary>The printer driver is known to harm the system.</summary>
    ERROR_PRINTER_DRIVER_BLOCKED = 3014,
    /// <summary>The specified printer driver package is currently in use.</summary>
    ERROR_PRINTER_DRIVER_PACKAGE_IN_USE = 3015,
    /// <summary>Unable to find a core driver package that is required by the printer driver package.</summary>
    ERROR_CORE_DRIVER_PACKAGE_NOT_FOUND = 3016,
    /// <summary>The requested operation failed. A system reboot is required to roll back changes made.</summary>
    ERROR_FAIL_REBOOT_REQUIRED = 3017,
    /// <summary>The requested operation failed. A system reboot has been initiated to roll back changes made.</summary>
    ERROR_FAIL_REBOOT_INITIATED = 3018,
    /// <summary>The specified printer driver was not found on the system and needs to be downloaded.</summary>
    ERROR_PRINTER_DRIVER_DOWNLOAD_NEEDED = 3019,
    /// <summary>The requested print job has failed to print. A print system update requires the job to be resubmitted.</summary>
    ERROR_PRINT_JOB_RESTART_REQUIRED = 3020,
    /// <summary>Reissue the given operation as a cached IO operation</summary>
    ERROR_IO_REISSUE_AS_CACHED = 3950,
    /// <summary>WINS encountered an error while processing the command.</summary>
    ERROR_WINS_INTERNAL = 4000,
    /// <summary>The local WINS cannot be deleted.</summary>
    ERROR_CAN_NOT_DEL_LOCAL_WINS = 4001,
    /// <summary>The importation from the file failed.</summary>
    ERROR_STATIC_INIT = 4002,
    /// <summary>The backup failed. Was a full backup done before?</summary>
    ERROR_INC_BACKUP = 4003,
    /// <summary>The backup failed. Check the directory to which you are backing the database.</summary>
    ERROR_FULL_BACKUP = 4004,
    /// <summary>The name does not exist in the WINS database.</summary>
    ERROR_REC_NON_EXISTENT = 4005,
    /// <summary>Replication with a nonconfigured partner is not allowed.</summary>
    ERROR_RPL_NOT_ALLOWED = 4006,
    /// <summary>The version of the supplied content information is not supported.</summary>
    PEERDIST_ERROR_CONTENTINFO_VERSION_UNSUPPORTED = 4050,
    /// <summary>The supplied content information is malformed.</summary>
    PEERDIST_ERROR_CANNOT_PARSE_CONTENTINFO = 4051,
    /// <summary>The requested data cannot be found in local or peer caches.</summary>
    PEERDIST_ERROR_MISSING_DATA = 4052,
    /// <summary>No more data is available or required.</summary>
    PEERDIST_ERROR_NO_MORE = 4053,
    /// <summary>The supplied object has not been initialized.</summary>
    PEERDIST_ERROR_NOT_INITIALIZED = 4054,
    /// <summary>The supplied object has already been initialized.</summary>
    PEERDIST_ERROR_ALREADY_INITIALIZED = 4055,
    /// <summary>A shutdown operation is already in progress.</summary>
    PEERDIST_ERROR_SHUTDOWN_IN_PROGRESS = 4056,
    /// <summary>The supplied object has already been invalidated.</summary>
    PEERDIST_ERROR_INVALIDATED = 4057,
    /// <summary>An element already exists and was not replaced.</summary>
    PEERDIST_ERROR_ALREADY_EXISTS = 4058,
    /// <summary>Can not cancel the requested operation as it has already been completed.</summary>
    PEERDIST_ERROR_OPERATION_NOTFOUND = 4059,
    /// <summary>Can not perform the reqested operation because it has already been carried out.</summary>
    PEERDIST_ERROR_ALREADY_COMPLETED = 4060,
    /// <summary>An operation accessed data beyond the bounds of valid data.</summary>
    PEERDIST_ERROR_OUT_OF_BOUNDS = 4061,
    /// <summary>The requested version is not supported.</summary>
    PEERDIST_ERROR_VERSION_UNSUPPORTED = 4062,
    /// <summary>A configuration value is invalid.</summary>
    PEERDIST_ERROR_INVALID_CONFIGURATION = 4063,
    /// <summary>The SKU is not licensed.</summary>
    PEERDIST_ERROR_NOT_LICENSED = 4064,
    /// <summary>PeerDist Service is still initializing and will be available shortly.</summary>
    PEERDIST_ERROR_SERVICE_UNAVAILABLE = 4065,
    /// <summary>The DHCP client has obtained an IP address that is already in use on the network. The local interface will be disabled until the DHCP client can obtain a new address.</summary>
    ERROR_DHCP_ADDRESS_CONFLICT = 4100,
    /// <summary>The GUID passed was not recognized as valid by a WMI data provider.</summary>
    ERROR_WMI_GUID_NOT_FOUND = 4200,
    /// <summary>The instance name passed was not recognized as valid by a WMI data provider.</summary>
    ERROR_WMI_INSTANCE_NOT_FOUND = 4201,
    /// <summary>The data item ID passed was not recognized as valid by a WMI data provider.</summary>
    ERROR_WMI_ITEMID_NOT_FOUND = 4202,
    /// <summary>The WMI request could not be completed and should be retried.</summary>
    ERROR_WMI_TRY_AGAIN = 4203,
    /// <summary>The WMI data provider could not be located.</summary>
    ERROR_WMI_DP_NOT_FOUND = 4204,
    /// <summary>The WMI data provider references an instance set that has not been registered.</summary>
    ERROR_WMI_UNRESOLVED_INSTANCE_REF = 4205,
    /// <summary>The WMI data block or event notification has already been enabled.</summary>
    ERROR_WMI_ALREADY_ENABLED = 4206,
    /// <summary>The WMI data block is no longer available.</summary>
    ERROR_WMI_GUID_DISCONNECTED = 4207,
    /// <summary>The WMI data service is not available.</summary>
    ERROR_WMI_SERVER_UNAVAILABLE = 4208,
    /// <summary>The WMI data provider failed to carry out the request.</summary>
    ERROR_WMI_DP_FAILED = 4209,
    /// <summary>The WMI MOF information is not valid.</summary>
    ERROR_WMI_INVALID_MOF = 4210,
    /// <summary>The WMI registration information is not valid.</summary>
    ERROR_WMI_INVALID_REGINFO = 4211,
    /// <summary>The WMI data block or event notification has already been disabled.</summary>
    ERROR_WMI_ALREADY_DISABLED = 4212,
    /// <summary>The WMI data item or data block is read only.</summary>
    ERROR_WMI_READ_ONLY = 4213,
    /// <summary>The WMI data item or data block could not be changed.</summary>
    ERROR_WMI_SET_FAILURE = 4214,
    /// <summary>The media identifier does not represent a valid medium.</summary>
    ERROR_INVALID_MEDIA = 4300,
    /// <summary>The library identifier does not represent a valid library.</summary>
    ERROR_INVALID_LIBRARY = 4301,
    /// <summary>The media pool identifier does not represent a valid media pool.</summary>
    ERROR_INVALID_MEDIA_POOL = 4302,
    /// <summary>The drive and medium are not compatible or exist in different libraries.</summary>
    ERROR_DRIVE_MEDIA_MISMATCH = 4303,
    /// <summary>The medium currently exists in an offline library and must be online to perform this operation.</summary>
    ERROR_MEDIA_OFFLINE = 4304,
    /// <summary>The operation cannot be performed on an offline library.</summary>
    ERROR_LIBRARY_OFFLINE = 4305,
    /// <summary>The library, drive, or media pool is empty.</summary>
    ERROR_EMPTY = 4306,
    /// <summary>The library, drive, or media pool must be empty to perform this operation.</summary>
    ERROR_NOT_EMPTY = 4307,
    /// <summary>No media is currently available in this media pool or library.</summary>
    ERROR_MEDIA_UNAVAILABLE = 4308,
    /// <summary>A resource required for this operation is disabled.</summary>
    ERROR_RESOURCE_DISABLED = 4309,
    /// <summary>The media identifier does not represent a valid cleaner.</summary>
    ERROR_INVALID_CLEANER = 4310,
    /// <summary>The drive cannot be cleaned or does not support cleaning.</summary>
    ERROR_UNABLE_TO_CLEAN = 4311,
    /// <summary>The object identifier does not represent a valid object.</summary>
    ERROR_OBJECT_NOT_FOUND = 4312,
    /// <summary>Unable to read from or write to the database.</summary>
    ERROR_DATABASE_FAILURE = 4313,
    /// <summary>The database is full.</summary>
    ERROR_DATABASE_FULL = 4314,
    /// <summary>The medium is not compatible with the device or media pool.</summary>
    ERROR_MEDIA_INCOMPATIBLE = 4315,
    /// <summary>The resource required for this operation does not exist.</summary>
    ERROR_RESOURCE_NOT_PRESENT = 4316,
    /// <summary>The operation identifier is not valid.</summary>
    ERROR_INVALID_OPERATION = 4317,
    /// <summary>The media is not mounted or ready for use.</summary>
    ERROR_MEDIA_NOT_AVAILABLE = 4318,
    /// <summary>The device is not ready for use.</summary>
    ERROR_DEVICE_NOT_AVAILABLE = 4319,
    /// <summary>The operator or administrator has refused the request.</summary>
    ERROR_REQUEST_REFUSED = 4320,
    /// <summary>The drive identifier does not represent a valid drive.</summary>
    ERROR_INVALID_DRIVE_OBJECT = 4321,
    /// <summary>Library is full. No slot is available for use.</summary>
    ERROR_LIBRARY_FULL = 4322,
    /// <summary>The transport cannot access the medium.</summary>
    ERROR_MEDIUM_NOT_ACCESSIBLE = 4323,
    /// <summary>Unable to load the medium into the drive.</summary>
    ERROR_UNABLE_TO_LOAD_MEDIUM = 4324,
    /// <summary>Unable to retrieve the drive status.</summary>
    ERROR_UNABLE_TO_INVENTORY_DRIVE = 4325,
    /// <summary>Unable to retrieve the slot status.</summary>
    ERROR_UNABLE_TO_INVENTORY_SLOT = 4326,
    /// <summary>Unable to retrieve status about the transport.</summary>
    ERROR_UNABLE_TO_INVENTORY_TRANSPORT = 4327,
    /// <summary>Cannot use the transport because it is already in use.</summary>
    ERROR_TRANSPORT_FULL = 4328,
    /// <summary>Unable to open or close the inject/eject port.</summary>
    ERROR_CONTROLLING_IEPORT = 4329,
    /// <summary>Unable to eject the medium because it is in a drive.</summary>
    ERROR_UNABLE_TO_EJECT_MOUNTED_MEDIA = 4330,
    /// <summary>A cleaner slot is already reserved.</summary>
    ERROR_CLEANER_SLOT_SET = 4331,
    /// <summary>A cleaner slot is not reserved.</summary>
    ERROR_CLEANER_SLOT_NOT_SET = 4332,
    /// <summary>The cleaner cartridge has performed the maximum number of drive cleanings.</summary>
    ERROR_CLEANER_CARTRIDGE_SPENT = 4333,
    /// <summary>Unexpected on-medium identifier.</summary>
    ERROR_UNEXPECTED_OMID = 4334,
    /// <summary>The last remaining item in this group or resource cannot be deleted.</summary>
    ERROR_CANT_DELETE_LAST_ITEM = 4335,
    /// <summary>The message provided exceeds the maximum size allowed for this parameter.</summary>
    ERROR_MESSAGE_EXCEEDS_MAX_SIZE = 4336,
    /// <summary>The volume contains system or paging files.</summary>
    ERROR_VOLUME_CONTAINS_SYS_FILES = 4337,
    /// <summary>The media type cannot be removed from this library since at least one drive in the library reports it can support this media type.</summary>
    ERROR_INDIGENOUS_TYPE = 4338,
    /// <summary>This offline media cannot be mounted on this system since no enabled drives are present which can be used.</summary>
    ERROR_NO_SUPPORTING_DRIVES = 4339,
    /// <summary>A cleaner cartridge is present in the tape library.</summary>
    ERROR_CLEANER_CARTRIDGE_INSTALLED = 4340,
    /// <summary>Cannot use the inject/eject port because it is not empty.</summary>
    ERROR_IEPORT_FULL = 4341,
    /// <summary>This file is currently not available for use on this computer.</summary>
    ERROR_FILE_OFFLINE = 4350,
    /// <summary>The remote storage service is not operational at this time.</summary>
    ERROR_REMOTE_STORAGE_NOT_ACTIVE = 4351,
    /// <summary>The remote storage service encountered a media error.</summary>
    ERROR_REMOTE_STORAGE_MEDIA_ERROR = 4352,
    /// <summary>The file or directory is not a reparse point.</summary>
    ERROR_NOT_A_REPARSE_POINT = 4390,
    /// <summary>The reparse point attribute cannot be set because it conflicts with an existing attribute.</summary>
    ERROR_REPARSE_ATTRIBUTE_CONFLICT = 4391,
    /// <summary>The data present in the reparse point buffer is invalid.</summary>
    ERROR_INVALID_REPARSE_DATA = 4392,
    /// <summary>The tag present in the reparse point buffer is invalid.</summary>
    ERROR_REPARSE_TAG_INVALID = 4393,
    /// <summary>There is a mismatch between the tag specified in the request and the tag present in the reparse point.</summary>
    ERROR_REPARSE_TAG_MISMATCH = 4394,
    /// <summary>Single Instance Storage is not available on this volume.</summary>
    ERROR_VOLUME_NOT_SIS_ENABLED = 4500,
    /// <summary>The operation cannot be completed because other resources are dependent on this resource.</summary>
    ERROR_DEPENDENT_RESOURCE_EXISTS = 5001,
    /// <summary>The cluster resource dependency cannot be found.</summary>
    ERROR_DEPENDENCY_NOT_FOUND = 5002,
    /// <summary>The cluster resource cannot be made dependent on the specified resource because it is already dependent.</summary>
    ERROR_DEPENDENCY_ALREADY_EXISTS = 5003,
    /// <summary>The cluster resource is not online.</summary>
    ERROR_RESOURCE_NOT_ONLINE = 5004,
    /// <summary>A cluster node is not available for this operation.</summary>
    ERROR_HOST_NODE_NOT_AVAILABLE = 5005,
    /// <summary>The cluster resource is not available.</summary>
    ERROR_RESOURCE_NOT_AVAILABLE = 5006,
    /// <summary>The cluster resource could not be found.</summary>
    ERROR_RESOURCE_NOT_FOUND = 5007,
    /// <summary>The cluster is being shut down.</summary>
    ERROR_SHUTDOWN_CLUSTER = 5008,
    /// <summary>A cluster node cannot be evicted from the cluster unless the node is down or it is the last node.</summary>
    ERROR_CANT_EVICT_ACTIVE_NODE = 5009,
    /// <summary>The object already exists.</summary>
    ERROR_OBJECT_ALREADY_EXISTS = 5010,
    /// <summary>The object is already in the list.</summary>
    ERROR_OBJECT_IN_LIST = 5011,
    /// <summary>The cluster group is not available for any new requests.</summary>
    ERROR_GROUP_NOT_AVAILABLE = 5012,
    /// <summary>The cluster group could not be found.</summary>
    ERROR_GROUP_NOT_FOUND = 5013,
    /// <summary>The operation could not be completed because the cluster group is not online.</summary>
    ERROR_GROUP_NOT_ONLINE = 5014,
    /// <summary>The operation failed because either the specified cluster node is not the owner of the resource, or the node is not a possible owner of the resource.</summary>
    ERROR_HOST_NODE_NOT_RESOURCE_OWNER = 5015,
    /// <summary>The operation failed because either the specified cluster node is not the owner of the group, or the node is not a possible owner of the group.</summary>
    ERROR_HOST_NODE_NOT_GROUP_OWNER = 5016,
    /// <summary>The cluster resource could not be created in the specified resource monitor.</summary>
    ERROR_RESMON_CREATE_FAILED = 5017,
    /// <summary>The cluster resource could not be brought online by the resource monitor.</summary>
    ERROR_RESMON_ONLINE_FAILED = 5018,
    /// <summary>The operation could not be completed because the cluster resource is online.</summary>
    ERROR_RESOURCE_ONLINE = 5019,
    /// <summary>The cluster resource could not be deleted or brought offline because it is the quorum resource.</summary>
    ERROR_QUORUM_RESOURCE = 5020,
    /// <summary>The cluster could not make the specified resource a quorum resource because it is not capable of being a quorum resource.</summary>
    ERROR_NOT_QUORUM_CAPABLE = 5021,
    /// <summary>The cluster software is shutting down.</summary>
    ERROR_CLUSTER_SHUTTING_DOWN = 5022,
    /// <summary>The group or resource is not in the correct state to perform the requested operation.</summary>
    ERROR_INVALID_STATE = 5023,
    /// <summary>The properties were stored but not all changes will take effect until the next time the resource is brought online.</summary>
    ERROR_RESOURCE_PROPERTIES_STORED = 5024,
    /// <summary>The cluster could not make the specified resource a quorum resource because it does not belong to a shared storage class.</summary>
    ERROR_NOT_QUORUM_CLASS = 5025,
    /// <summary>The cluster resource could not be deleted since it is a core resource.</summary>
    ERROR_CORE_RESOURCE = 5026,
    /// <summary>The quorum resource failed to come online.</summary>
    ERROR_QUORUM_RESOURCE_ONLINE_FAILED = 5027,
    /// <summary>The quorum log could not be created or mounted successfully.</summary>
    ERROR_QUORUMLOG_OPEN_FAILED = 5028,
    /// <summary>The cluster log is corrupt.</summary>
    ERROR_CLUSTERLOG_CORRUPT = 5029,
    /// <summary>The record could not be written to the cluster log since it exceeds the maximum size.</summary>
    ERROR_CLUSTERLOG_RECORD_EXCEEDS_MAXSIZE = 5030,
    /// <summary>The cluster log exceeds its maximum size.</summary>
    ERROR_CLUSTERLOG_EXCEEDS_MAXSIZE = 5031,
    /// <summary>No checkpoint record was found in the cluster log.</summary>
    ERROR_CLUSTERLOG_CHKPOINT_NOT_FOUND = 5032,
    /// <summary>The minimum required disk space needed for logging is not available.</summary>
    ERROR_CLUSTERLOG_NOT_ENOUGH_SPACE = 5033,
    /// <summary>The cluster node failed to take control of the quorum resource because the resource is owned by another active node.</summary>
    ERROR_QUORUM_OWNER_ALIVE = 5034,
    /// <summary>A cluster network is not available for this operation.</summary>
    ERROR_NETWORK_NOT_AVAILABLE = 5035,
    /// <summary>A cluster node is not available for this operation.</summary>
    ERROR_NODE_NOT_AVAILABLE = 5036,
    /// <summary>All cluster nodes must be running to perform this operation.</summary>
    ERROR_ALL_NODES_NOT_AVAILABLE = 5037,
    /// <summary>A cluster resource failed.</summary>
    ERROR_RESOURCE_FAILED = 5038,
    /// <summary>The cluster node is not valid.</summary>
    ERROR_CLUSTER_INVALID_NODE = 5039,
    /// <summary>The cluster node already exists.</summary>
    ERROR_CLUSTER_NODE_EXISTS = 5040,
    /// <summary>A node is in the process of joining the cluster.</summary>
    ERROR_CLUSTER_JOIN_IN_PROGRESS = 5041,
    /// <summary>The cluster node was not found.</summary>
    ERROR_CLUSTER_NODE_NOT_FOUND = 5042,
    /// <summary>The cluster local node information was not found.</summary>
    ERROR_CLUSTER_LOCAL_NODE_NOT_FOUND = 5043,
    /// <summary>The cluster network already exists.</summary>
    ERROR_CLUSTER_NETWORK_EXISTS = 5044,
    /// <summary>The cluster network was not found.</summary>
    ERROR_CLUSTER_NETWORK_NOT_FOUND = 5045,
    /// <summary>The cluster network interface already exists.</summary>
    ERROR_CLUSTER_NETINTERFACE_EXISTS = 5046,
    /// <summary>The cluster network interface was not found.</summary>
    ERROR_CLUSTER_NETINTERFACE_NOT_FOUND = 5047,
    /// <summary>The cluster request is not valid for this object.</summary>
    ERROR_CLUSTER_INVALID_REQUEST = 5048,
    /// <summary>The cluster network provider is not valid.</summary>
    ERROR_CLUSTER_INVALID_NETWORK_PROVIDER = 5049,
    /// <summary>The cluster node is down.</summary>
    ERROR_CLUSTER_NODE_DOWN = 5050,
    /// <summary>The cluster node is not reachable.</summary>
    ERROR_CLUSTER_NODE_UNREACHABLE = 5051,
    /// <summary>The cluster node is not a member of the cluster.</summary>
    ERROR_CLUSTER_NODE_NOT_MEMBER = 5052,
    /// <summary>A cluster join operation is not in progress.</summary>
    ERROR_CLUSTER_JOIN_NOT_IN_PROGRESS = 5053,
    /// <summary>The cluster network is not valid.</summary>
    ERROR_CLUSTER_INVALID_NETWORK = 5054,
    /// <summary>The cluster node is up.</summary>
    ERROR_CLUSTER_NODE_UP = 5056,
    /// <summary>The cluster IP address is already in use.</summary>
    ERROR_CLUSTER_IPADDR_IN_USE = 5057,
    /// <summary>The cluster node is not paused.</summary>
    ERROR_CLUSTER_NODE_NOT_PAUSED = 5058,
    /// <summary>No cluster security context is available.</summary>
    ERROR_CLUSTER_NO_SECURITY_CONTEXT = 5059,
    /// <summary>The cluster network is not configured for internal cluster communication.</summary>
    ERROR_CLUSTER_NETWORK_NOT_INTERNAL = 5060,
    /// <summary>The cluster node is already up.</summary>
    ERROR_CLUSTER_NODE_ALREADY_UP = 5061,
    /// <summary>The cluster node is already down.</summary>
    ERROR_CLUSTER_NODE_ALREADY_DOWN = 5062,
    /// <summary>The cluster network is already online.</summary>
    ERROR_CLUSTER_NETWORK_ALREADY_ONLINE = 5063,
    /// <summary>The cluster network is already offline.</summary>
    ERROR_CLUSTER_NETWORK_ALREADY_OFFLINE = 5064,
    /// <summary>The cluster node is already a member of the cluster.</summary>
    ERROR_CLUSTER_NODE_ALREADY_MEMBER = 5065,
    /// <summary>The cluster network is the only one configured for internal cluster communication between two or more active cluster nodes. The internal communication capability cannot be removed from the network.</summary>
    ERROR_CLUSTER_LAST_INTERNAL_NETWORK = 5066,
    /// <summary>One or more cluster resources depend on the network to provide service to clients. The client access capability cannot be removed from the network.</summary>
    ERROR_CLUSTER_NETWORK_HAS_DEPENDENTS = 5067,
    /// <summary>This operation cannot be performed on the cluster resource as it the quorum resource. You may not bring the quorum resource offline or modify its possible owners list.</summary>
    ERROR_INVALID_OPERATION_ON_QUORUM = 5068,
    /// <summary>The cluster quorum resource is not allowed to have any dependencies.</summary>
    ERROR_DEPENDENCY_NOT_ALLOWED = 5069,
    /// <summary>The cluster node is paused.</summary>
    ERROR_CLUSTER_NODE_PAUSED = 5070,
    /// <summary>The cluster resource cannot be brought online. The owner node cannot run this resource.</summary>
    ERROR_NODE_CANT_HOST_RESOURCE = 5071,
    /// <summary>The cluster node is not ready to perform the requested operation.</summary>
    ERROR_CLUSTER_NODE_NOT_READY = 5072,
    /// <summary>The cluster node is shutting down.</summary>
    ERROR_CLUSTER_NODE_SHUTTING_DOWN = 5073,
    /// <summary>The cluster join operation was aborted.</summary>
    ERROR_CLUSTER_JOIN_ABORTED = 5074,
    /// <summary>The cluster join operation failed due to incompatible software versions between the joining node and its sponsor.</summary>
    ERROR_CLUSTER_INCOMPATIBLE_VERSIONS = 5075,
    /// <summary>This resource cannot be created because the cluster has reached the limit on the number of resources it can monitor.</summary>
    ERROR_CLUSTER_MAXNUM_OF_RESOURCES_EXCEEDED = 5076,
    /// <summary>The system configuration changed during the cluster join or form operation. The join or form operation was aborted.</summary>
    ERROR_CLUSTER_SYSTEM_CONFIG_CHANGED = 5077,
    /// <summary>The specified resource type was not found.</summary>
    ERROR_CLUSTER_RESOURCE_TYPE_NOT_FOUND = 5078,
    /// <summary>The specified node does not support a resource of this type. This may be due to version inconsistencies or due to the absence of the resource DLL on this node.</summary>
    ERROR_CLUSTER_RESTYPE_NOT_SUPPORTED = 5079,
    /// <summary>The specified resource name is not supported by this resource DLL. This may be due to a bad (or changed) name supplied to the resource DLL.</summary>
    ERROR_CLUSTER_RESNAME_NOT_FOUND = 5080,
    /// <summary>No authentication package could be registered with the RPC server.</summary>
    ERROR_CLUSTER_NO_RPC_PACKAGES_REGISTERED = 5081,
    /// <summary>You cannot bring the group online because the owner of the group is not in the preferred list for the group. To change the owner node for the group, move the group.</summary>
    ERROR_CLUSTER_OWNER_NOT_IN_PREFLIST = 5082,
    /// <summary>The join operation failed because the cluster database sequence number has changed or is incompatible with the locker node. This may happen during a join operation if the cluster database was changing during the join.</summary>
    ERROR_CLUSTER_DATABASE_SEQMISMATCH = 5083,
    /// <summary>The resource monitor will not allow the fail operation to be performed while the resource is in its current state. This may happen if the resource is in a pending state.</summary>
    ERROR_RESMON_INVALID_STATE = 5084,
    /// <summary>A non locker code got a request to reserve the lock for making global updates.</summary>
    ERROR_CLUSTER_GUM_NOT_LOCKER = 5085,
    /// <summary>The quorum disk could not be located by the cluster service.</summary>
    ERROR_QUORUM_DISK_NOT_FOUND = 5086,
    /// <summary>The backed up cluster database is possibly corrupt.</summary>
    ERROR_DATABASE_BACKUP_CORRUPT = 5087,
    /// <summary>A DFS root already exists in this cluster node.</summary>
    ERROR_CLUSTER_NODE_ALREADY_HAS_DFS_ROOT = 5088,
    /// <summary>An attempt to modify a resource property failed because it conflicts with another existing property.</summary>
    ERROR_RESOURCE_PROPERTY_UNCHANGEABLE = 5089,
    /// <summary>An operation was attempted that is incompatible with the current membership state of the node.</summary>
    ERROR_CLUSTER_MEMBERSHIP_INVALID_STATE = 5890,
    /// <summary>The quorum resource does not contain the quorum log.</summary>
    ERROR_CLUSTER_QUORUMLOG_NOT_FOUND = 5891,
    /// <summary>The membership engine requested shutdown of the cluster service on this node.</summary>
    ERROR_CLUSTER_MEMBERSHIP_HALT = 5892,
    /// <summary>The join operation failed because the cluster instance ID of the joining node does not match the cluster instance ID of the sponsor node.</summary>
    ERROR_CLUSTER_INSTANCE_ID_MISMATCH = 5893,
    /// <summary>A matching cluster network for the specified IP address could not be found.</summary>
    ERROR_CLUSTER_NETWORK_NOT_FOUND_FOR_IP = 5894,
    /// <summary>The actual data type of the property did not match the expected data type of the property.</summary>
    ERROR_CLUSTER_PROPERTY_DATA_TYPE_MISMATCH = 5895,
    /// <summary>The cluster node was evicted from the cluster successfully, but the node was not cleaned up. To determine what cleanup steps failed and how to recover, see the Failover Clustering application event log using Event Viewer.</summary>
    ERROR_CLUSTER_EVICT_WITHOUT_CLEANUP = 5896,
    /// <summary>Two or more parameter values specified for a resource&#39;s properties are in conflict.</summary>
    ERROR_CLUSTER_PARAMETER_MISMATCH = 5897,
    /// <summary>This computer cannot be made a member of a cluster.</summary>
    ERROR_NODE_CANNOT_BE_CLUSTERED = 5898,
    /// <summary>This computer cannot be made a member of a cluster because it does not have the correct version of Windows installed.</summary>
    ERROR_CLUSTER_WRONG_OS_VERSION = 5899,
    /// <summary>A cluster cannot be created with the specified cluster name because that cluster name is already in use. Specify a different name for the cluster.</summary>
    ERROR_CLUSTER_CANT_CREATE_DUP_CLUSTER_NAME = 5900,
    /// <summary>The cluster configuration action has already been committed.</summary>
    ERROR_CLUSCFG_ALREADY_COMMITTED = 5901,
    /// <summary>The cluster configuration action could not be rolled back.</summary>
    ERROR_CLUSCFG_ROLLBACK_FAILED = 5902,
    /// <summary>The drive letter assigned to a system disk on one node conflicted with the drive letter assigned to a disk on another node.</summary>
    ERROR_CLUSCFG_SYSTEM_DISK_DRIVE_LETTER_CONFLICT = 5903,
    /// <summary>One or more nodes in the cluster are running a version of Windows that does not support this operation.</summary>
    ERROR_CLUSTER_OLD_VERSION = 5904,
    /// <summary>The name of the corresponding computer account doesn&#39;t match the Network Name for this resource.</summary>
    ERROR_CLUSTER_MISMATCHED_COMPUTER_ACCT_NAME = 5905,
    /// <summary>No network adapters are available.</summary>
    ERROR_CLUSTER_NO_NET_ADAPTERS = 5906,
    /// <summary>The cluster node has been poisoned.</summary>
    ERROR_CLUSTER_POISONED = 5907,
    /// <summary>The group is unable to accept the request since it is moving to another node.</summary>
    ERROR_CLUSTER_GROUP_MOVING = 5908,
    /// <summary>The resource type cannot accept the request since is too busy performing another operation.</summary>
    ERROR_CLUSTER_RESOURCE_TYPE_BUSY = 5909,
    /// <summary>The call to the cluster resource DLL timed out.</summary>
    ERROR_RESOURCE_CALL_TIMED_OUT = 5910,
    /// <summary>The address is not valid for an IPv6 Address resource. A global IPv6 address is required, and it must match a cluster network. Compatibility addresses are not permitted.</summary>
    ERROR_INVALID_CLUSTER_IPV6_ADDRESS = 5911,
    /// <summary>An internal cluster error occurred. A call to an invalid function was attempted.</summary>
    ERROR_CLUSTER_INTERNAL_INVALID_FUNCTION = 5912,
    /// <summary>A parameter value is out of acceptable range.</summary>
    ERROR_CLUSTER_PARAMETER_OUT_OF_BOUNDS = 5913,
    /// <summary>A network error occurred while sending data to another node in the cluster. The number of bytes transmitted was less than required.</summary>
    ERROR_CLUSTER_PARTIAL_SEND = 5914,
    /// <summary>An invalid cluster registry operation was attempted.</summary>
    ERROR_CLUSTER_REGISTRY_INVALID_FUNCTION = 5915,
    /// <summary>An input string of characters is not properly terminated.</summary>
    ERROR_CLUSTER_INVALID_STRING_TERMINATION = 5916,
    /// <summary>An input string of characters is not in a valid format for the data it represents.</summary>
    ERROR_CLUSTER_INVALID_STRING_FORMAT = 5917,
    /// <summary>An internal cluster error occurred. A cluster database transaction was attempted while a transaction was already in progress.</summary>
    ERROR_CLUSTER_DATABASE_TRANSACTION_IN_PROGRESS = 5918,
    /// <summary>An internal cluster error occurred. There was an attempt to commit a cluster database transaction while no transaction was in progress.</summary>
    ERROR_CLUSTER_DATABASE_TRANSACTION_NOT_IN_PROGRESS = 5919,
    /// <summary>An internal cluster error occurred. Data was not properly initialized.</summary>
    ERROR_CLUSTER_NULL_DATA = 5920,
    /// <summary>An error occurred while reading from a stream of data. An unexpected number of bytes was returned.</summary>
    ERROR_CLUSTER_PARTIAL_READ = 5921,
    /// <summary>An error occurred while writing to a stream of data. The required number of bytes could not be written.</summary>
    ERROR_CLUSTER_PARTIAL_WRITE = 5922,
    /// <summary>An error occurred while deserializing a stream of cluster data.</summary>
    ERROR_CLUSTER_CANT_DESERIALIZE_DATA = 5923,
    /// <summary>One or more property values for this resource are in conflict with one or more property values associated with its dependent resource(s).</summary>
    ERROR_DEPENDENT_RESOURCE_PROPERTY_CONFLICT = 5924,
    /// <summary>A quorum of cluster nodes was not present to form a cluster.</summary>
    ERROR_CLUSTER_NO_QUORUM = 5925,
    /// <summary>The cluster network is not valid for an IPv6 Address resource, or it does not match the configured address.</summary>
    ERROR_CLUSTER_INVALID_IPV6_NETWORK = 5926,
    /// <summary>The cluster network is not valid for an IPv6 Tunnel resource. Check the configuration of the IP Address resource on which the IPv6 Tunnel resource depends.</summary>
    ERROR_CLUSTER_INVALID_IPV6_TUNNEL_NETWORK = 5927,
    /// <summary>Quorum resource cannot reside in the Available Storage group.</summary>
    ERROR_QUORUM_NOT_ALLOWED_IN_THIS_GROUP = 5928,
    /// <summary>The dependencies for this resource are nested too deeply.</summary>
    ERROR_DEPENDENCY_TREE_TOO_COMPLEX = 5929,
    /// <summary>The call into the resource DLL raised an unhandled exception.</summary>
    ERROR_EXCEPTION_IN_RESOURCE_CALL = 5930,
    /// <summary>The RHS process failed to initialize.</summary>
    ERROR_CLUSTER_RHS_FAILED_INITIALIZATION = 5931,
    /// <summary>The Failover Clustering feature is not installed on this node.</summary>
    ERROR_CLUSTER_NOT_INSTALLED = 5932,
    /// <summary>The resources must be online on the same node for this operation</summary>
    ERROR_CLUSTER_RESOURCES_MUST_BE_ONLINE_ON_THE_SAME_NODE = 5933,
    /// <summary>A new node can not be added since this cluster is already at its maximum number of nodes.</summary>
    ERROR_CLUSTER_MAX_NODES_IN_CLUSTER = 5934,
    /// <summary>This cluster can not be created since the specified number of nodes exceeds the maximum allowed limit.</summary>
    ERROR_CLUSTER_TOO_MANY_NODES = 5935,
    /// <summary>An attempt to use the specified cluster name failed because an enabled computer object with the given name already exists in the domain.</summary>
    ERROR_CLUSTER_OBJECT_ALREADY_USED = 5936,
    /// <summary>This cluster cannot be destroyed. It has non-core application groups which must be deleted before the cluster can be destroyed.</summary>
    ERROR_NONCORE_GROUPS_FOUND = 5937,
    /// <summary>File share associated with file share witness resource cannot be hosted by this cluster or any of its nodes.</summary>
    ERROR_FILE_SHARE_RESOURCE_CONFLICT = 5938,
    /// <summary>Eviction of this node is invalid at this time. Due to quorum requirements node eviction will result in cluster shutdown. If it is the last node in the cluster, destroy cluster command should be used.</summary>
    ERROR_CLUSTER_EVICT_INVALID_REQUEST = 5939,
    /// <summary>Only one instance of this resource type is allowed in the cluster.</summary>
    ERROR_CLUSTER_SINGLETON_RESOURCE = 5940,
    /// <summary>Only one instance of this resource type is allowed per resource group.</summary>
    ERROR_CLUSTER_GROUP_SINGLETON_RESOURCE = 5941,
    /// <summary>The resource failed to come online due to the failure of one or more provider resources.</summary>
    ERROR_CLUSTER_RESOURCE_PROVIDER_FAILED = 5942,
    /// <summary>The resource has indicated that it cannot come online on any node.</summary>
    ERROR_CLUSTER_RESOURCE_CONFIGURATION_ERROR = 5943,
    /// <summary>The current operation cannot be performed on this group at this time.</summary>
    ERROR_CLUSTER_GROUP_BUSY = 5944,
    /// <summary>The directory or file is not located on a cluster shared volume.</summary>
    ERROR_CLUSTER_NOT_SHARED_VOLUME = 5945,
    /// <summary>The Security Descriptor does not meet the requirements for a cluster.</summary>
    ERROR_CLUSTER_INVALID_SECURITY_DESCRIPTOR = 5946,
    /// <summary>There is one or more shared volumes resources configured in the cluster. Those resources must be moved to available storage in order for operation to succeed.</summary>
    ERROR_CLUSTER_SHARED_VOLUMES_IN_USE = 5947,
    /// <summary>This group or resource cannot be directly manipulated. Use shared volume APIs to perform desired operation.</summary>
    ERROR_CLUSTER_USE_SHARED_VOLUMES_API = 5948,
    /// <summary>Back up is in progress. Please wait for backup completion before trying this operation again.</summary>
    ERROR_CLUSTER_BACKUP_IN_PROGRESS = 5949,
    /// <summary>The path does not belong to a cluster shared volume.</summary>
    ERROR_NON_CSV_PATH = 5950,
    /// <summary>The cluster shared volume is not locally mounted on this node.</summary>
    ERROR_CSV_VOLUME_NOT_LOCAL = 5951,
    /// <summary>The cluster watchdog is terminating.</summary>
    ERROR_CLUSTER_WATCHDOG_TERMINATING = 5952,
    /// <summary>The specified file could not be encrypted.</summary>
    ERROR_ENCRYPTION_FAILED = 6000,
    /// <summary>The specified file could not be decrypted.</summary>
    ERROR_DECRYPTION_FAILED = 6001,
    /// <summary>The specified file is encrypted and the user does not have the ability to decrypt it.</summary>
    ERROR_FILE_ENCRYPTED = 6002,
    /// <summary>There is no valid encryption recovery policy configured for this system.</summary>
    ERROR_NO_RECOVERY_POLICY = 6003,
    /// <summary>The required encryption driver is not loaded for this system.</summary>
    ERROR_NO_EFS = 6004,
    /// <summary>The file was encrypted with a different encryption driver than is currently loaded.</summary>
    ERROR_WRONG_EFS = 6005,
    /// <summary>There are no EFS keys defined for the user.</summary>
    ERROR_NO_USER_KEYS = 6006,
    /// <summary>The specified file is not encrypted.</summary>
    ERROR_FILE_NOT_ENCRYPTED = 6007,
    /// <summary>The specified file is not in the defined EFS export format.</summary>
    ERROR_NOT_EXPORT_FORMAT = 6008,
    /// <summary>The specified file is read only.</summary>
    ERROR_FILE_READ_ONLY = 6009,
    /// <summary>The directory has been disabled for encryption.</summary>
    ERROR_DIR_EFS_DISALLOWED = 6010,
    /// <summary>The server is not trusted for remote encryption operation.</summary>
    ERROR_EFS_SERVER_NOT_TRUSTED = 6011,
    /// <summary>Recovery policy configured for this system contains invalid recovery certificate.</summary>
    ERROR_BAD_RECOVERY_POLICY = 6012,
    /// <summary>The encryption algorithm used on the source file needs a bigger key buffer than the one on the destination file.</summary>
    ERROR_EFS_ALG_BLOB_TOO_BIG = 6013,
    /// <summary>The disk partition does not support file encryption.</summary>
    ERROR_VOLUME_NOT_SUPPORT_EFS = 6014,
    /// <summary>This machine is disabled for file encryption.</summary>
    ERROR_EFS_DISABLED = 6015,
    /// <summary>A newer system is required to decrypt this encrypted file.</summary>
    ERROR_EFS_VERSION_NOT_SUPPORT = 6016,
    /// <summary>The remote server sent an invalid response for a file being opened with Client Side Encryption.</summary>
    ERROR_CS_ENCRYPTION_INVALID_SERVER_RESPONSE = 6017,
    /// <summary>Client Side Encryption is not supported by the remote server even though it claims to support it.</summary>
    ERROR_CS_ENCRYPTION_UNSUPPORTED_SERVER = 6018,
    /// <summary>File is encrypted and should be opened in Client Side Encryption mode.</summary>
    ERROR_CS_ENCRYPTION_EXISTING_ENCRYPTED_FILE = 6019,
    /// <summary>A new encrypted file is being created and a $EFS needs to be provided.</summary>
    ERROR_CS_ENCRYPTION_NEW_ENCRYPTED_FILE = 6020,
    /// <summary>The SMB client requested a CSE FSCTL on a non-CSE file.</summary>
    ERROR_CS_ENCRYPTION_FILE_NOT_CSE = 6021,
    /// <summary>The requested operation was blocked by policy. For more information, contact your system administrator.</summary>
    ERROR_ENCRYPTION_POLICY_DENIES_OPERATION = 6022,
    /// <summary>The list of servers for this workgroup is not currently available</summary>
    ERROR_NO_BROWSER_SERVERS_FOUND = 6118,
    /// <summary>The Task Scheduler service must be configured to run in the System account to function properly. Individual tasks may be configured to run in other accounts.</summary>
    SCHED_E_SERVICE_NOT_LOCALSYSTEM = 6200,
    /// <summary>Log service encountered an invalid log sector.</summary>
    ERROR_LOG_SECTOR_INVALID = 6600,
    /// <summary>Log service encountered a log sector with invalid block parity.</summary>
    ERROR_LOG_SECTOR_PARITY_INVALID = 6601,
    /// <summary>Log service encountered a remapped log sector.</summary>
    ERROR_LOG_SECTOR_REMAPPED = 6602,
    /// <summary>Log service encountered a partial or incomplete log block.</summary>
    ERROR_LOG_BLOCK_INCOMPLETE = 6603,
    /// <summary>Log service encountered an attempt access data outside the active log range.</summary>
    ERROR_LOG_INVALID_RANGE = 6604,
    /// <summary>Log service user marshalling buffers are exhausted.</summary>
    ERROR_LOG_BLOCKS_EXHAUSTED = 6605,
    /// <summary>Log service encountered an attempt read from a marshalling area with an invalid read context.</summary>
    ERROR_LOG_READ_CONTEXT_INVALID = 6606,
    /// <summary>Log service encountered an invalid log restart area.</summary>
    ERROR_LOG_RESTART_INVALID = 6607,
    /// <summary>Log service encountered an invalid log block version.</summary>
    ERROR_LOG_BLOCK_VERSION = 6608,
    /// <summary>Log service encountered an invalid log block.</summary>
    ERROR_LOG_BLOCK_INVALID = 6609,
    /// <summary>Log service encountered an attempt to read the log with an invalid read mode.</summary>
    ERROR_LOG_READ_MODE_INVALID = 6610,
    /// <summary>Log service encountered a log stream with no restart area.</summary>
    ERROR_LOG_NO_RESTART = 6611,
    /// <summary>Log service encountered a corrupted metadata file.</summary>
    ERROR_LOG_METADATA_CORRUPT = 6612,
    /// <summary>Log service encountered a metadata file that could not be created by the log file system.</summary>
    ERROR_LOG_METADATA_INVALID = 6613,
    /// <summary>Log service encountered a metadata file with inconsistent data.</summary>
    ERROR_LOG_METADATA_INCONSISTENT = 6614,
    /// <summary>Log service encountered an attempt to erroneous allocate or dispose reservation space.</summary>
    ERROR_LOG_RESERVATION_INVALID = 6615,
    /// <summary>Log service cannot delete log file or file system container.</summary>
    ERROR_LOG_CANT_DELETE = 6616,
    /// <summary>Log service has reached the maximum allowable containers allocated to a log file.</summary>
    ERROR_LOG_CONTAINER_LIMIT_EXCEEDED = 6617,
    /// <summary>Log service has attempted to read or write backward past the start of the log.</summary>
    ERROR_LOG_START_OF_LOG = 6618,
    /// <summary>Log policy could not be installed because a policy of the same type is already present.</summary>
    ERROR_LOG_POLICY_ALREADY_INSTALLED = 6619,
    /// <summary>Log policy in question was not installed at the time of the request.</summary>
    ERROR_LOG_POLICY_NOT_INSTALLED = 6620,
    /// <summary>The installed set of policies on the log is invalid.</summary>
    ERROR_LOG_POLICY_INVALID = 6621,
    /// <summary>A policy on the log in question prevented the operation from completing.</summary>
    ERROR_LOG_POLICY_CONFLICT = 6622,
    /// <summary>Log space cannot be reclaimed because the log is pinned by the archive tail.</summary>
    ERROR_LOG_PINNED_ARCHIVE_TAIL = 6623,
    /// <summary>Log record is not a record in the log file.</summary>
    ERROR_LOG_RECORD_NONEXISTENT = 6624,
    /// <summary>Number of reserved log records or the adjustment of the number of reserved log records is invalid.</summary>
    ERROR_LOG_RECORDS_RESERVED_INVALID = 6625,
    /// <summary>Reserved log space or the adjustment of the log space is invalid.</summary>
    ERROR_LOG_SPACE_RESERVED_INVALID = 6626,
    /// <summary>An new or existing archive tail or base of the active log is invalid.</summary>
    ERROR_LOG_TAIL_INVALID = 6627,
    /// <summary>Log space is exhausted.</summary>
    ERROR_LOG_FULL = 6628,
    /// <summary>The log could not be set to the requested size.</summary>
    ERROR_COULD_NOT_RESIZE_LOG = 6629,
    /// <summary>Log is multiplexed, no direct writes to the physical log is allowed.</summary>
    ERROR_LOG_MULTIPLEXED = 6630,
    /// <summary>The operation failed because the log is a dedicated log.</summary>
    ERROR_LOG_DEDICATED = 6631,
    /// <summary>The operation requires an archive context.</summary>
    ERROR_LOG_ARCHIVE_NOT_IN_PROGRESS = 6632,
    /// <summary>Log archival is in progress.</summary>
    ERROR_LOG_ARCHIVE_IN_PROGRESS = 6633,
    /// <summary>The operation requires a non-ephemeral log, but the log is ephemeral.</summary>
    ERROR_LOG_EPHEMERAL = 6634,
    /// <summary>The log must have at least two containers before it can be read from or written to.</summary>
    ERROR_LOG_NOT_ENOUGH_CONTAINERS = 6635,
    /// <summary>A log client has already registered on the stream.</summary>
    ERROR_LOG_CLIENT_ALREADY_REGISTERED = 6636,
    /// <summary>A log client has not been registered on the stream.</summary>
    ERROR_LOG_CLIENT_NOT_REGISTERED = 6637,
    /// <summary>A request has already been made to handle the log full condition.</summary>
    ERROR_LOG_FULL_HANDLER_IN_PROGRESS = 6638,
    /// <summary>Log service encountered an error when attempting to read from a log container.</summary>
    ERROR_LOG_CONTAINER_READ_FAILED = 6639,
    /// <summary>Log service encountered an error when attempting to write to a log container.</summary>
    ERROR_LOG_CONTAINER_WRITE_FAILED = 6640,
    /// <summary>Log service encountered an error when attempting open a log container.</summary>
    ERROR_LOG_CONTAINER_OPEN_FAILED = 6641,
    /// <summary>Log service encountered an invalid container state when attempting a requested action.</summary>
    ERROR_LOG_CONTAINER_STATE_INVALID = 6642,
    /// <summary>Log service is not in the correct state to perform a requested action.</summary>
    ERROR_LOG_STATE_INVALID = 6643,
    /// <summary>Log space cannot be reclaimed because the log is pinned.</summary>
    ERROR_LOG_PINNED = 6644,
    /// <summary>Log metadata flush failed.</summary>
    ERROR_LOG_METADATA_FLUSH_FAILED = 6645,
    /// <summary>Security on the log and its containers is inconsistent.</summary>
    ERROR_LOG_INCONSISTENT_SECURITY = 6646,
    /// <summary>Records were appended to the log or reservation changes were made, but the log could not be flushed.</summary>
    ERROR_LOG_APPENDED_FLUSH_FAILED = 6647,
    /// <summary>The log is pinned due to reservation consuming most of the log space. Free some reserved records to make space available.</summary>
    ERROR_LOG_PINNED_RESERVATION = 6648,
    /// <summary>The transaction handle associated with this operation is not valid.</summary>
    ERROR_INVALID_TRANSACTION = 6700,
    /// <summary>The requested operation was made in the context of a transaction that is no longer active.</summary>
    ERROR_TRANSACTION_NOT_ACTIVE = 6701,
    /// <summary>The requested operation is not valid on the Transaction object in its current state.</summary>
    ERROR_TRANSACTION_REQUEST_NOT_VALID = 6702,
    /// <summary>The caller has called a response API, but the response is not expected because the TM did not issue the corresponding request to the caller.</summary>
    ERROR_TRANSACTION_NOT_REQUESTED = 6703,
    /// <summary>It is too late to perform the requested operation, since the Transaction has already been aborted.</summary>
    ERROR_TRANSACTION_ALREADY_ABORTED = 6704,
    /// <summary>It is too late to perform the requested operation, since the Transaction has already been committed.</summary>
    ERROR_TRANSACTION_ALREADY_COMMITTED = 6705,
    /// <summary>The Transaction Manager was unable to be successfully initialized. Transacted operations are not supported.</summary>
    ERROR_TM_INITIALIZATION_FAILED = 6706,
    /// <summary>The specified ResourceManager made no changes or updates to the resource under this transaction.</summary>
    ERROR_RESOURCEMANAGER_READ_ONLY = 6707,
    /// <summary>The resource manager has attempted to prepare a transaction that it has not successfully joined.</summary>
    ERROR_TRANSACTION_NOT_JOINED = 6708,
    /// <summary>The Transaction object already has a superior enlistment, and the caller attempted an operation that would have created a new superior. Only a single superior enlistment is allow.</summary>
    ERROR_TRANSACTION_SUPERIOR_EXISTS = 6709,
    /// <summary>The RM tried to register a protocol that already exists.</summary>
    ERROR_CRM_PROTOCOL_ALREADY_EXISTS = 6710,
    /// <summary>The attempt to propagate the Transaction failed.</summary>
    ERROR_TRANSACTION_PROPAGATION_FAILED = 6711,
    /// <summary>The requested propagation protocol was not registered as a CRM.</summary>
    ERROR_CRM_PROTOCOL_NOT_FOUND = 6712,
    /// <summary>The buffer passed in to PushTransaction or PullTransaction is not in a valid format.</summary>
    ERROR_TRANSACTION_INVALID_MARSHALL_BUFFER = 6713,
    /// <summary>The current transaction context associated with the thread is not a valid handle to a transaction object.</summary>
    ERROR_CURRENT_TRANSACTION_NOT_VALID = 6714,
    /// <summary>The specified Transaction object could not be opened, because it was not found.</summary>
    ERROR_TRANSACTION_NOT_FOUND = 6715,
    /// <summary>The specified ResourceManager object could not be opened, because it was not found.</summary>
    ERROR_RESOURCEMANAGER_NOT_FOUND = 6716,
    /// <summary>The specified Enlistment object could not be opened, because it was not found.</summary>
    ERROR_ENLISTMENT_NOT_FOUND = 6717,
    /// <summary>The specified TransactionManager object could not be opened, because it was not found.</summary>
    ERROR_TRANSACTIONMANAGER_NOT_FOUND = 6718,
    /// <summary>The object specified could not be created or opened, because its associated TransactionManager is not online.  The TransactionManager must be brought fully Online by calling RecoverTransactionManager to recover to the end of its LogFile before objects in its Transaction or ResourceManager namespaces can be opened.  In addition, errors in writing records to its LogFile can cause a TransactionManager to go offline.</summary>
    ERROR_TRANSACTIONMANAGER_NOT_ONLINE = 6719,
    /// <summary>The specified TransactionManager was unable to create the objects contained in its logfile in the Ob namespace. Therefore, the TransactionManager was unable to recover.</summary>
    ERROR_TRANSACTIONMANAGER_RECOVERY_NAME_COLLISION = 6720,
    /// <summary>The call to create a superior Enlistment on this Transaction object could not be completed, because the Transaction object specified for the enlistment is a subordinate branch of the Transaction. Only the root of the Transaction can be enlisted on as a superior.</summary>
    ERROR_TRANSACTION_NOT_ROOT = 6721,
    /// <summary>Because the associated transaction manager or resource manager has been closed, the handle is no longer valid.</summary>
    ERROR_TRANSACTION_OBJECT_EXPIRED = 6722,
    /// <summary>The specified operation could not be performed on this Superior enlistment, because the enlistment was not created with the corresponding completion response in the NotificationMask.</summary>
    ERROR_TRANSACTION_RESPONSE_NOT_ENLISTED = 6723,
    /// <summary>The specified operation could not be performed, because the record that would be logged was too long. This can occur because of two conditions: either there are too many Enlistments on this Transaction, or the combined RecoveryInformation being logged on behalf of those Enlistments is too long.</summary>
    ERROR_TRANSACTION_RECORD_TOO_LONG = 6724,
    /// <summary>Implicit transaction are not supported.</summary>
    ERROR_IMPLICIT_TRANSACTION_NOT_SUPPORTED = 6725,
    /// <summary>The kernel transaction manager had to abort or forget the transaction because it blocked forward progress.</summary>
    ERROR_TRANSACTION_INTEGRITY_VIOLATED = 6726,
    /// <summary>The TransactionManager identity that was supplied did not match the one recorded in the TransactionManager&#39;s log file.</summary>
    ERROR_TRANSACTIONMANAGER_IDENTITY_MISMATCH = 6727,
    /// <summary>This snapshot operation cannot continue because a transactional resource manager cannot be frozen in its current state.  Please try again.</summary>
    ERROR_RM_CANNOT_BE_FROZEN_FOR_SNAPSHOT = 6728,
    /// <summary>The transaction cannot be enlisted on with the specified EnlistmentMask, because the transaction has already completed the PrePrepare phase.  In order to ensure correctness, the ResourceManager must switch to a write-through mode and cease caching data within this transaction.  Enlisting for only subsequent transaction phases may still succeed.</summary>
    ERROR_TRANSACTION_MUST_WRITETHROUGH = 6729,
    /// <summary>The transaction does not have a superior enlistment.</summary>
    ERROR_TRANSACTION_NO_SUPERIOR = 6730,
    /// <summary>The attempt to commit the Transaction completed, but it is possible that some portion of the transaction tree did not commit successfully due to heuristics.  Therefore it is possible that some data modified in the transaction may not have committed, resulting in transactional inconsistency.  If possible, check the consistency of the associated data.</summary>
    ERROR_HEURISTIC_DAMAGE_POSSIBLE = 6731,
    /// <summary>The function attempted to use a name that is reserved for use by another transaction.</summary>
    ERROR_TRANSACTIONAL_CONFLICT = 6800,
    /// <summary>Transaction support within the specified resource manager is not started or was shut down due to an error.</summary>
    ERROR_RM_NOT_ACTIVE = 6801,
    /// <summary>The metadata of the RM has been corrupted. The RM will not function.</summary>
    ERROR_RM_METADATA_CORRUPT = 6802,
    /// <summary>The specified directory does not contain a resource manager.</summary>
    ERROR_DIRECTORY_NOT_RM = 6803,
    /// <summary>The remote server or share does not support transacted file operations.</summary>
    ERROR_TRANSACTIONS_UNSUPPORTED_REMOTE = 6805,
    /// <summary>The requested log size is invalid.</summary>
    ERROR_LOG_RESIZE_INVALID_SIZE = 6806,
    /// <summary>The object (file, stream, link) corresponding to the handle has been deleted by a Transaction Savepoint Rollback.</summary>
    ERROR_OBJECT_NO_LONGER_EXISTS = 6807,
    /// <summary>The specified file miniversion was not found for this transacted file open.</summary>
    ERROR_STREAM_MINIVERSION_NOT_FOUND = 6808,
    /// <summary>The specified file miniversion was found but has been invalidated. Most likely cause is a transaction savepoint rollback.</summary>
    ERROR_STREAM_MINIVERSION_NOT_VALID = 6809,
    /// <summary>A miniversion may only be opened in the context of the transaction that created it.</summary>
    ERROR_MINIVERSION_INACCESSIBLE_FROM_SPECIFIED_TRANSACTION = 6810,
    /// <summary>It is not possible to open a miniversion with modify access.</summary>
    ERROR_CANT_OPEN_MINIVERSION_WITH_MODIFY_INTENT = 6811,
    /// <summary>It is not possible to create any more miniversions for this stream.</summary>
    ERROR_CANT_CREATE_MORE_STREAM_MINIVERSIONS = 6812,
    /// <summary>The remote server sent mismatching version number or Fid for a file opened with transactions.</summary>
    ERROR_REMOTE_FILE_VERSION_MISMATCH = 6814,
    /// <summary>The handle has been invalidated by a transaction. The most likely cause is the presence of memory mapping on a file or an open handle when the transaction ended or rolled back to savepoint.</summary>
    ERROR_HANDLE_NO_LONGER_VALID = 6815,
    /// <summary>There is no transaction metadata on the file.</summary>
    ERROR_NO_TXF_METADATA = 6816,
    /// <summary>The log data is corrupt.</summary>
    ERROR_LOG_CORRUPTION_DETECTED = 6817,
    /// <summary>The file can&#39;t be recovered because there is a handle still open on it.</summary>
    ERROR_CANT_RECOVER_WITH_HANDLE_OPEN = 6818,
    /// <summary>The transaction outcome is unavailable because the resource manager responsible for it has disconnected.</summary>
    ERROR_RM_DISCONNECTED = 6819,
    /// <summary>The request was rejected because the enlistment in question is not a superior enlistment.</summary>
    ERROR_ENLISTMENT_NOT_SUPERIOR = 6820,
    /// <summary>The transactional resource manager is already consistent. Recovery is not needed.</summary>
    ERROR_RECOVERY_NOT_NEEDED = 6821,
    /// <summary>The transactional resource manager has already been started.</summary>
    ERROR_RM_ALREADY_STARTED = 6822,
    /// <summary>The file cannot be opened transactionally, because its identity depends on the outcome of an unresolved transaction.</summary>
    ERROR_FILE_IDENTITY_NOT_PERSISTENT = 6823,
    /// <summary>The operation cannot be performed because another transaction is depending on the fact that this property will not change.</summary>
    ERROR_CANT_BREAK_TRANSACTIONAL_DEPENDENCY = 6824,
    /// <summary>The operation would involve a single file with two transactional resource managers and is therefore not allowed.</summary>
    ERROR_CANT_CROSS_RM_BOUNDARY = 6825,
    /// <summary>The $Txf directory must be empty for this operation to succeed.</summary>
    ERROR_TXF_DIR_NOT_EMPTY = 6826,
    /// <summary>The operation would leave a transactional resource manager in an inconsistent state and is therefore not allowed.</summary>
    ERROR_INDOUBT_TRANSACTIONS_EXIST = 6827,
    /// <summary>The operation could not be completed because the transaction manager does not have a log.</summary>
    ERROR_TM_VOLATILE = 6828,
    /// <summary>A rollback could not be scheduled because a previously scheduled rollback has already executed or been queued for execution.</summary>
    ERROR_ROLLBACK_TIMER_EXPIRED = 6829,
    /// <summary>The transactional metadata attribute on the file or directory is corrupt and unreadable.</summary>
    ERROR_TXF_ATTRIBUTE_CORRUPT = 6830,
    /// <summary>The encryption operation could not be completed because a transaction is active.</summary>
    ERROR_EFS_NOT_ALLOWED_IN_TRANSACTION = 6831,
    /// <summary>This object is not allowed to be opened in a transaction.</summary>
    ERROR_TRANSACTIONAL_OPEN_NOT_ALLOWED = 6832,
    /// <summary>An attempt to create space in the transactional resource manager&#39;s log failed. The failure status has been recorded in the event log.</summary>
    ERROR_LOG_GROWTH_FAILED = 6833,
    /// <summary>Memory mapping (creating a mapped section) a remote file under a transaction is not supported.</summary>
    ERROR_TRANSACTED_MAPPING_UNSUPPORTED_REMOTE = 6834,
    /// <summary>Transaction metadata is already present on this file and cannot be superseded.</summary>
    ERROR_TXF_METADATA_ALREADY_PRESENT = 6835,
    /// <summary>A transaction scope could not be entered because the scope handler has not been initialized.</summary>
    ERROR_TRANSACTION_SCOPE_CALLBACKS_NOT_SET = 6836,
    /// <summary>Promotion was required in order to allow the resource manager to enlist, but the transaction was set to disallow it.</summary>
    ERROR_TRANSACTION_REQUIRED_PROMOTION = 6837,
    /// <summary>This file is open for modification in an unresolved transaction and may be opened for execute only by a transacted reader.</summary>
    ERROR_CANNOT_EXECUTE_FILE_IN_TRANSACTION = 6838,
    /// <summary>The request to thaw frozen transactions was ignored because transactions had not previously been frozen.</summary>
    ERROR_TRANSACTIONS_NOT_FROZEN = 6839,
    /// <summary>Transactions cannot be frozen because a freeze is already in progress.</summary>
    ERROR_TRANSACTION_FREEZE_IN_PROGRESS = 6840,
    /// <summary>The target volume is not a snapshot volume. This operation is only valid on a volume mounted as a snapshot.</summary>
    ERROR_NOT_SNAPSHOT_VOLUME = 6841,
    /// <summary>The savepoint operation failed because files are open on the transaction. This is not permitted.</summary>
    ERROR_NO_SAVEPOINT_WITH_OPEN_FILES = 6842,
    /// <summary>Windows has discovered corruption in a file, and that file has since been repaired. Data loss may have occurred.</summary>
    ERROR_DATA_LOST_REPAIR = 6843,
    /// <summary>The sparse operation could not be completed because a transaction is active on the file.</summary>
    ERROR_SPARSE_NOT_ALLOWED_IN_TRANSACTION = 6844,
    /// <summary>The call to create a TransactionManager object failed because the Tm Identity stored in the logfile does not match the Tm Identity that was passed in as an argument.</summary>
    ERROR_TM_IDENTITY_MISMATCH = 6845,
    /// <summary>I/O was attempted on a section object that has been floated as a result of a transaction ending. There is no valid data.</summary>
    ERROR_FLOATED_SECTION = 6846,
    /// <summary>The transactional resource manager cannot currently accept transacted work due to a transient condition such as low resources.</summary>
    ERROR_CANNOT_ACCEPT_TRANSACTED_WORK = 6847,
    /// <summary>The transactional resource manager had too many tranactions outstanding that could not be aborted. The transactional resource manger has been shut down.</summary>
    ERROR_CANNOT_ABORT_TRANSACTIONS = 6848,
    /// <summary>The operation could not be completed due to bad clusters on disk.</summary>
    ERROR_BAD_CLUSTERS = 6849,
    /// <summary>The compression operation could not be completed because a transaction is active on the file.</summary>
    ERROR_COMPRESSION_NOT_ALLOWED_IN_TRANSACTION = 6850,
    /// <summary>The operation could not be completed because the volume is dirty. Please run chkdsk and try again.</summary>
    ERROR_VOLUME_DIRTY = 6851,
    /// <summary>The link tracking operation could not be completed because a transaction is active.</summary>
    ERROR_NO_LINK_TRACKING_IN_TRANSACTION = 6852,
    /// <summary>This operation cannot be performed in a transaction.</summary>
    ERROR_OPERATION_NOT_SUPPORTED_IN_TRANSACTION = 6853,
    /// <summary>The handle is no longer properly associated with its transaction.  It may have been opened in a transactional resource manager that was subsequently forced to restart.  Please close the handle and open a new one.</summary>
    ERROR_EXPIRED_HANDLE = 6854,
    /// <summary>The specified operation could not be performed because the resource manager is not enlisted in the transaction.</summary>
    ERROR_TRANSACTION_NOT_ENLISTED = 6855,
    /// <summary>The specified session name is invalid.</summary>
    ERROR_CTX_WINSTATION_NAME_INVALID = 7001,
    /// <summary>The specified protocol driver is invalid.</summary>
    ERROR_CTX_INVALID_PD = 7002,
    /// <summary>The specified protocol driver was not found in the system path.</summary>
    ERROR_CTX_PD_NOT_FOUND = 7003,
    /// <summary>The specified terminal connection driver was not found in the system path.</summary>
    ERROR_CTX_WD_NOT_FOUND = 7004,
    /// <summary>A registry key for event logging could not be created for this session.</summary>
    ERROR_CTX_CANNOT_MAKE_EVENTLOG_ENTRY = 7005,
    /// <summary>A service with the same name already exists on the system.</summary>
    ERROR_CTX_SERVICE_NAME_COLLISION = 7006,
    /// <summary>A close operation is pending on the session.</summary>
    ERROR_CTX_CLOSE_PENDING = 7007,
    /// <summary>There are no free output buffers available.</summary>
    ERROR_CTX_NO_OUTBUF = 7008,
    /// <summary>The MODEM.INF file was not found.</summary>
    ERROR_CTX_MODEM_INF_NOT_FOUND = 7009,
    /// <summary>The modem name was not found in MODEM.INF.</summary>
    ERROR_CTX_INVALID_MODEMNAME = 7010,
    /// <summary>The modem did not accept the command sent to it. Verify that the configured modem name matches the attached modem.</summary>
    ERROR_CTX_MODEM_RESPONSE_ERROR = 7011,
    /// <summary>The modem did not respond to the command sent to it. Verify that the modem is properly cabled and powered on.</summary>
    ERROR_CTX_MODEM_RESPONSE_TIMEOUT = 7012,
    /// <summary>Carrier detect has failed or carrier has been dropped due to disconnect.</summary>
    ERROR_CTX_MODEM_RESPONSE_NO_CARRIER = 7013,
    /// <summary>Dial tone not detected within the required time. Verify that the phone cable is properly attached and functional.</summary>
    ERROR_CTX_MODEM_RESPONSE_NO_DIALTONE = 7014,
    /// <summary>Busy signal detected at remote site on callback.</summary>
    ERROR_CTX_MODEM_RESPONSE_BUSY = 7015,
    /// <summary>Voice detected at remote site on callback.</summary>
    ERROR_CTX_MODEM_RESPONSE_VOICE = 7016,
    /// <summary>Transport driver error</summary>
    ERROR_CTX_TD_ERROR = 7017,
    /// <summary>The specified session cannot be found.</summary>
    ERROR_CTX_WINSTATION_NOT_FOUND = 7022,
    /// <summary>The specified session name is already in use.</summary>
    ERROR_CTX_WINSTATION_ALREADY_EXISTS = 7023,
    /// <summary>The task you are trying to do can&#39;t be completed because Remote Desktop Services is currently busy. Please try again in a few minutes. Other users should still be able to log on.</summary>
    ERROR_CTX_WINSTATION_BUSY = 7024,
    /// <summary>An attempt has been made to connect to a session whose video mode is not supported by the current client.</summary>
    ERROR_CTX_BAD_VIDEO_MODE = 7025,
    /// <summary>The application attempted to enable DOS graphics mode. DOS graphics mode is not supported.</summary>
    ERROR_CTX_GRAPHICS_INVALID = 7035,
    /// <summary>Your interactive logon privilege has been disabled. Please contact your administrator.</summary>
    ERROR_CTX_LOGON_DISABLED = 7037,
    /// <summary>The requested operation can be performed only on the system console. This is most often the result of a driver or system DLL requiring direct console access.</summary>
    ERROR_CTX_NOT_CONSOLE = 7038,
    /// <summary>The client failed to respond to the server connect message.</summary>
    ERROR_CTX_CLIENT_QUERY_TIMEOUT = 7040,
    /// <summary>Disconnecting the console session is not supported.</summary>
    ERROR_CTX_CONSOLE_DISCONNECT = 7041,
    /// <summary>Reconnecting a disconnected session to the console is not supported.</summary>
    ERROR_CTX_CONSOLE_CONNECT = 7042,
    /// <summary>The request to control another session remotely was denied.</summary>
    ERROR_CTX_SHADOW_DENIED = 7044,
    /// <summary>The requested session access is denied.</summary>
    ERROR_CTX_WINSTATION_ACCESS_DENIED = 7045,
    /// <summary>The specified terminal connection driver is invalid.</summary>
    ERROR_CTX_INVALID_WD = 7049,
    /// <summary>The requested session cannot be controlled remotely. This may be because the session is disconnected or does not currently have a user logged on.</summary>
    ERROR_CTX_SHADOW_INVALID = 7050,
    /// <summary>The requested session is not configured to allow remote control.</summary>
    ERROR_CTX_SHADOW_DISABLED = 7051,
    /// <summary>Your request to connect to this Terminal Server has been rejected. Your Terminal Server client license number is currently being used by another user. Please call your system administrator to obtain a unique license number.</summary>
    ERROR_CTX_CLIENT_LICENSE_IN_USE = 7052,
    /// <summary>Your request to connect to this Terminal Server has been rejected. Your Terminal Server client license number has not been entered for this copy of the Terminal Server client. Please contact your system administrator.</summary>
    ERROR_CTX_CLIENT_LICENSE_NOT_SET = 7053,
    /// <summary>The number of connections to this computer is limited and all connections are in use right now. Try connecting later or contact your system administrator.</summary>
    ERROR_CTX_LICENSE_NOT_AVAILABLE = 7054,
    /// <summary>The client you are using is not licensed to use this system. Your logon request is denied.</summary>
    ERROR_CTX_LICENSE_CLIENT_INVALID = 7055,
    /// <summary>The system license has expired. Your logon request is denied.</summary>
    ERROR_CTX_LICENSE_EXPIRED = 7056,
    /// <summary>Remote control could not be terminated because the specified session is not currently being remotely controlled.</summary>
    ERROR_CTX_SHADOW_NOT_RUNNING = 7057,
    /// <summary>The remote control of the console was terminated because the display mode was changed. Changing the display mode in a remote control session is not supported.</summary>
    ERROR_CTX_SHADOW_ENDED_BY_MODE_CHANGE = 7058,
    /// <summary>Activation has already been reset the maximum number of times for this installation. Your activation timer will not be cleared.</summary>
    ERROR_ACTIVATION_COUNT_EXCEEDED = 7059,
    /// <summary>Remote logins are currently disabled.</summary>
    ERROR_CTX_WINSTATIONS_DISABLED = 7060,
    /// <summary>You do not have the proper encryption level to access this Session.</summary>
    ERROR_CTX_ENCRYPTION_LEVEL_REQUIRED = 7061,
    /// <summary>The user %s\\%s is currently logged on to this computer. Only the current user or an administrator can log on to this computer.</summary>
    ERROR_CTX_SESSION_IN_USE = 7062,
    /// <summary>The user %s\\%s is already logged on to the console of this computer. You do not have permission to log in at this time. To resolve this issue, contact %s\\%s and have them log off.</summary>
    ERROR_CTX_NO_FORCE_LOGOFF = 7063,
    /// <summary>Unable to log you on because of an account restriction.</summary>
    ERROR_CTX_ACCOUNT_RESTRICTION = 7064,
    /// <summary>The RDP protocol component %2 detected an error in the protocol stream and has disconnected the client.</summary>
    ERROR_RDP_PROTOCOL_ERROR = 7065,
    /// <summary>The Client Drive Mapping Service Has Connected on Terminal Connection.</summary>
    ERROR_CTX_CDM_CONNECT = 7066,
    /// <summary>The Client Drive Mapping Service Has Disconnected on Terminal Connection.</summary>
    ERROR_CTX_CDM_DISCONNECT = 7067,
    /// <summary>The Terminal Server security layer detected an error in the protocol stream and has disconnected the client.</summary>
    ERROR_CTX_SECURITY_LAYER_ERROR = 7068,
    /// <summary>The target session is incompatible with the current session.</summary>
    ERROR_TS_INCOMPATIBLE_SESSIONS = 7069,
    /// <summary>Windows can&#39;t connect to your session because a problem occurred in the Windows video subsystem. Try connecting again later, or contact the server administrator for assistance.</summary>
    ERROR_TS_VIDEO_SUBSYSTEM_ERROR = 7070,
    /// <summary>The file replication service API was called incorrectly.</summary>
    FRS_ERR_INVALID_API_SEQUENCE = 8001,
    /// <summary>The file replication service cannot be started.</summary>
    FRS_ERR_STARTING_SERVICE = 8002,
    /// <summary>The file replication service cannot be stopped.</summary>
    FRS_ERR_STOPPING_SERVICE = 8003,
    /// <summary>The file replication service API terminated the request. The event log may have more information.</summary>
    FRS_ERR_INTERNAL_API = 8004,
    /// <summary>The file replication service terminated the request. The event log may have more information.</summary>
    FRS_ERR_INTERNAL = 8005,
    /// <summary>The file replication service cannot be contacted. The event log may have more information.</summary>
    FRS_ERR_SERVICE_COMM = 8006,
    /// <summary>The file replication service cannot satisfy the request because the user has insufficient privileges. The event log may have more information.</summary>
    FRS_ERR_INSUFFICIENT_PRIV = 8007,
    /// <summary>The file replication service cannot satisfy the request because authenticated RPC is not available. The event log may have more information.</summary>
    FRS_ERR_AUTHENTICATION = 8008,
    /// <summary>The file replication service cannot satisfy the request because the user has insufficient privileges on the domain controller. The event log may have more information.</summary>
    FRS_ERR_PARENT_INSUFFICIENT_PRIV = 8009,
    /// <summary>The file replication service cannot satisfy the request because authenticated RPC is not available on the domain controller. The event log may have more information.</summary>
    FRS_ERR_PARENT_AUTHENTICATION = 8010,
    /// <summary>The file replication service cannot communicate with the file replication service on the domain controller. The event log may have more information.</summary>
    FRS_ERR_CHILD_TO_PARENT_COMM = 8011,
    /// <summary>The file replication service on the domain controller cannot communicate with the file replication service on this computer. The event log may have more information.</summary>
    FRS_ERR_PARENT_TO_CHILD_COMM = 8012,
    /// <summary>The file replication service cannot populate the system volume because of an internal error. The event log may have more information.</summary>
    FRS_ERR_SYSVOL_POPULATE = 8013,
    /// <summary>The file replication service cannot populate the system volume because of an internal timeout. The event log may have more information.</summary>
    FRS_ERR_SYSVOL_POPULATE_TIMEOUT = 8014,
    /// <summary>The file replication service cannot process the request. The system volume is busy with a previous request.</summary>
    FRS_ERR_SYSVOL_IS_BUSY = 8015,
    /// <summary>The file replication service cannot stop replicating the system volume because of an internal error. The event log may have more information.</summary>
    FRS_ERR_SYSVOL_DEMOTE = 8016,
    /// <summary>The file replication service detected an invalid parameter.</summary>
    FRS_ERR_INVALID_SERVICE_PARAMETER = 8017,
    /// <summary>An error occurred while installing the directory service. For more information, see the event log.</summary>
    ERROR_DS_NOT_INSTALLED = 8200,
    /// <summary>The directory service evaluated group memberships locally.</summary>
    ERROR_DS_MEMBERSHIP_EVALUATED_LOCALLY = 8201,
    /// <summary>The specified directory service attribute or value does not exist.</summary>
    ERROR_DS_NO_ATTRIBUTE_OR_VALUE = 8202,
    /// <summary>The attribute syntax specified to the directory service is invalid.</summary>
    ERROR_DS_INVALID_ATTRIBUTE_SYNTAX = 8203,
    /// <summary>The attribute type specified to the directory service is not defined.</summary>
    ERROR_DS_ATTRIBUTE_TYPE_UNDEFINED = 8204,
    /// <summary>The specified directory service attribute or value already exists.</summary>
    ERROR_DS_ATTRIBUTE_OR_VALUE_EXISTS = 8205,
    /// <summary>The directory service is busy.</summary>
    ERROR_DS_BUSY = 8206,
    /// <summary>The directory service is unavailable.</summary>
    ERROR_DS_UNAVAILABLE = 8207,
    /// <summary>The directory service was unable to allocate a relative identifier.</summary>
    ERROR_DS_NO_RIDS_ALLOCATED = 8208,
    /// <summary>The directory service has exhausted the pool of relative identifiers.</summary>
    ERROR_DS_NO_MORE_RIDS = 8209,
    /// <summary>The requested operation could not be performed because the directory service is not the master for that type of operation.</summary>
    ERROR_DS_INCORRECT_ROLE_OWNER = 8210,
    /// <summary>The directory service was unable to initialize the subsystem that allocates relative identifiers.</summary>
    ERROR_DS_RIDMGR_INIT_ERROR = 8211,
    /// <summary>The requested operation did not satisfy one or more constraints associated with the class of the object.</summary>
    ERROR_DS_OBJ_CLASS_VIOLATION = 8212,
    /// <summary>The directory service can perform the requested operation only on a leaf object.</summary>
    ERROR_DS_CANT_ON_NON_LEAF = 8213,
    /// <summary>The directory service cannot perform the requested operation on the RDN attribute of an object.</summary>
    ERROR_DS_CANT_ON_RDN = 8214,
    /// <summary>The directory service detected an attempt to modify the object class of an object.</summary>
    ERROR_DS_CANT_MOD_OBJ_CLASS = 8215,
    /// <summary>The requested cross-domain move operation could not be performed.</summary>
    ERROR_DS_CROSS_DOM_MOVE_ERROR = 8216,
    /// <summary>Unable to contact the global catalog server.</summary>
    ERROR_DS_GC_NOT_AVAILABLE = 8217,
    /// <summary>The policy object is shared and can only be modified at the root.</summary>
    ERROR_SHARED_POLICY = 8218,
    /// <summary>The policy object does not exist.</summary>
    ERROR_POLICY_OBJECT_NOT_FOUND = 8219,
    /// <summary>The requested policy information is only in the directory service.</summary>
    ERROR_POLICY_ONLY_IN_DS = 8220,
    /// <summary>A domain controller promotion is currently active.</summary>
    ERROR_PROMOTION_ACTIVE = 8221,
    /// <summary>A domain controller promotion is not currently active</summary>
    ERROR_NO_PROMOTION_ACTIVE = 8222,
    /// <summary>An operations error occurred.</summary>
    ERROR_DS_OPERATIONS_ERROR = 8224,
    /// <summary>A protocol error occurred.</summary>
    ERROR_DS_PROTOCOL_ERROR = 8225,
    /// <summary>The time limit for this request was exceeded.</summary>
    ERROR_DS_TIMELIMIT_EXCEEDED = 8226,
    /// <summary>The size limit for this request was exceeded.</summary>
    ERROR_DS_SIZELIMIT_EXCEEDED = 8227,
    /// <summary>The administrative limit for this request was exceeded.</summary>
    ERROR_DS_ADMIN_LIMIT_EXCEEDED = 8228,
    /// <summary>The compare response was false.</summary>
    ERROR_DS_COMPARE_FALSE = 8229,
    /// <summary>The compare response was true.</summary>
    ERROR_DS_COMPARE_TRUE = 8230,
    /// <summary>The requested authentication method is not supported by the server.</summary>
    ERROR_DS_AUTH_METHOD_NOT_SUPPORTED = 8231,
    /// <summary>A more secure authentication method is required for this server.</summary>
    ERROR_DS_STRONG_AUTH_REQUIRED = 8232,
    /// <summary>Inappropriate authentication.</summary>
    ERROR_DS_INAPPROPRIATE_AUTH = 8233,
    /// <summary>The authentication mechanism is unknown.</summary>
    ERROR_DS_AUTH_UNKNOWN = 8234,
    /// <summary>A referral was returned from the server.</summary>
    ERROR_DS_REFERRAL = 8235,
    /// <summary>The server does not support the requested critical extension.</summary>
    ERROR_DS_UNAVAILABLE_CRIT_EXTENSION = 8236,
    /// <summary>This request requires a secure connection.</summary>
    ERROR_DS_CONFIDENTIALITY_REQUIRED = 8237,
    /// <summary>Inappropriate matching.</summary>
    ERROR_DS_INAPPROPRIATE_MATCHING = 8238,
    /// <summary>A constraint violation occurred.</summary>
    ERROR_DS_CONSTRAINT_VIOLATION = 8239,
    /// <summary>There is no such object on the server.</summary>
    ERROR_DS_NO_SUCH_OBJECT = 8240,
    /// <summary>There is an alias problem.</summary>
    ERROR_DS_ALIAS_PROBLEM = 8241,
    /// <summary>An invalid dn syntax has been specified.</summary>
    ERROR_DS_INVALID_DN_SYNTAX = 8242,
    /// <summary>The object is a leaf object.</summary>
    ERROR_DS_IS_LEAF = 8243,
    /// <summary>There is an alias dereferencing problem.</summary>
    ERROR_DS_ALIAS_DEREF_PROBLEM = 8244,
    /// <summary>The server is unwilling to process the request.</summary>
    ERROR_DS_UNWILLING_TO_PERFORM = 8245,
    /// <summary>A loop has been detected.</summary>
    ERROR_DS_LOOP_DETECT = 8246,
    /// <summary>There is a naming violation.</summary>
    ERROR_DS_NAMING_VIOLATION = 8247,
    /// <summary>The result set is too large.</summary>
    ERROR_DS_OBJECT_RESULTS_TOO_LARGE = 8248,
    /// <summary>The operation affects multiple DSAs</summary>
    ERROR_DS_AFFECTS_MULTIPLE_DSAS = 8249,
    /// <summary>The server is not operational.</summary>
    ERROR_DS_SERVER_DOWN = 8250,
    /// <summary>A local error has occurred.</summary>
    ERROR_DS_LOCAL_ERROR = 8251,
    /// <summary>An encoding error has occurred.</summary>
    ERROR_DS_ENCODING_ERROR = 8252,
    /// <summary>A decoding error has occurred.</summary>
    ERROR_DS_DECODING_ERROR = 8253,
    /// <summary>The search filter cannot be recognized.</summary>
    ERROR_DS_FILTER_UNKNOWN = 8254,
    /// <summary>One or more parameters are illegal.</summary>
    ERROR_DS_PARAM_ERROR = 8255,
    /// <summary>The specified method is not supported.</summary>
    ERROR_DS_NOT_SUPPORTED = 8256,
    /// <summary>No results were returned.</summary>
    ERROR_DS_NO_RESULTS_RETURNED = 8257,
    /// <summary>The specified control is not supported by the server.</summary>
    ERROR_DS_CONTROL_NOT_FOUND = 8258,
    /// <summary>A referral loop was detected by the client.</summary>
    ERROR_DS_CLIENT_LOOP = 8259,
    /// <summary>The preset referral limit was exceeded.</summary>
    ERROR_DS_REFERRAL_LIMIT_EXCEEDED = 8260,
    /// <summary>The search requires a SORT control.</summary>
    ERROR_DS_SORT_CONTROL_MISSING = 8261,
    /// <summary>The search results exceed the offset range specified.</summary>
    ERROR_DS_OFFSET_RANGE_ERROR = 8262,
    /// <summary>The root object must be the head of a naming context. The root object cannot have an instantiated parent.</summary>
    ERROR_DS_ROOT_MUST_BE_NC = 8301,
    /// <summary>The add replica operation cannot be performed. The naming context must be writeable in order to create the replica.</summary>
    ERROR_DS_ADD_REPLICA_INHIBITED = 8302,
    /// <summary>A reference to an attribute that is not defined in the schema occurred.</summary>
    ERROR_DS_ATT_NOT_DEF_IN_SCHEMA = 8303,
    /// <summary>The maximum size of an object has been exceeded.</summary>
    ERROR_DS_MAX_OBJ_SIZE_EXCEEDED = 8304,
    /// <summary>An attempt was made to add an object to the directory with a name that is already in use.</summary>
    ERROR_DS_OBJ_STRING_NAME_EXISTS = 8305,
    /// <summary>An attempt was made to add an object of a class that does not have an RDN defined in the schema.</summary>
    ERROR_DS_NO_RDN_DEFINED_IN_SCHEMA = 8306,
    /// <summary>An attempt was made to add an object using an RDN that is not the RDN defined in the schema.</summary>
    ERROR_DS_RDN_DOESNT_MATCH_SCHEMA = 8307,
    /// <summary>None of the requested attributes were found on the objects.</summary>
    ERROR_DS_NO_REQUESTED_ATTS_FOUND = 8308,
    /// <summary>The user buffer is too small.</summary>
    ERROR_DS_USER_BUFFER_TO_SMALL = 8309,
    /// <summary>The attribute specified in the operation is not present on the object.</summary>
    ERROR_DS_ATT_IS_NOT_ON_OBJ = 8310,
    /// <summary>Illegal modify operation. Some aspect of the modification is not permitted.</summary>
    ERROR_DS_ILLEGAL_MOD_OPERATION = 8311,
    /// <summary>The specified object is too large.</summary>
    ERROR_DS_OBJ_TOO_LARGE = 8312,
    /// <summary>The specified instance type is not valid.</summary>
    ERROR_DS_BAD_INSTANCE_TYPE = 8313,
    /// <summary>The operation must be performed at a master DSA.</summary>
    ERROR_DS_MASTERDSA_REQUIRED = 8314,
    /// <summary>The object class attribute must be specified.</summary>
    ERROR_DS_OBJECT_CLASS_REQUIRED = 8315,
    /// <summary>A required attribute is missing.</summary>
    ERROR_DS_MISSING_REQUIRED_ATT = 8316,
    /// <summary>An attempt was made to modify an object to include an attribute that is not legal for its class.</summary>
    ERROR_DS_ATT_NOT_DEF_FOR_CLASS = 8317,
    /// <summary>The specified attribute is already present on the object.</summary>
    ERROR_DS_ATT_ALREADY_EXISTS = 8318,
    /// <summary>The specified attribute is not present, or has no values.</summary>
    ERROR_DS_CANT_ADD_ATT_VALUES = 8320,
    /// <summary>Multiple values were specified for an attribute that can have only one value.</summary>
    ERROR_DS_SINGLE_VALUE_CONSTRAINT = 8321,
    /// <summary>A value for the attribute was not in the acceptable range of values.</summary>
    ERROR_DS_RANGE_CONSTRAINT = 8322,
    /// <summary>The specified value already exists.</summary>
    ERROR_DS_ATT_VAL_ALREADY_EXISTS = 8323,
    /// <summary>The attribute cannot be removed because it is not present on the object.</summary>
    ERROR_DS_CANT_REM_MISSING_ATT = 8324,
    /// <summary>The attribute value cannot be removed because it is not present on the object.</summary>
    ERROR_DS_CANT_REM_MISSING_ATT_VAL = 8325,
    /// <summary>The specified root object cannot be a subref.</summary>
    ERROR_DS_ROOT_CANT_BE_SUBREF = 8326,
    /// <summary>Chaining is not permitted.</summary>
    ERROR_DS_NO_CHAINING = 8327,
    /// <summary>Chained evaluation is not permitted.</summary>
    ERROR_DS_NO_CHAINED_EVAL = 8328,
    /// <summary>The operation could not be performed because the object&#39;s parent is either uninstantiated or deleted.</summary>
    ERROR_DS_NO_PARENT_OBJECT = 8329,
    /// <summary>Having a parent that is an alias is not permitted. Aliases are leaf objects.</summary>
    ERROR_DS_PARENT_IS_AN_ALIAS = 8330,
    /// <summary>The object and parent must be of the same type, either both masters or both replicas.</summary>
    ERROR_DS_CANT_MIX_MASTER_AND_REPS = 8331,
    /// <summary>The operation cannot be performed because child objects exist. This operation can only be performed on a leaf object.</summary>
    ERROR_DS_CHILDREN_EXIST = 8332,
    /// <summary>Directory object not found.</summary>
    ERROR_DS_OBJ_NOT_FOUND = 8333,
    /// <summary>The aliased object is missing.</summary>
    ERROR_DS_ALIASED_OBJ_MISSING = 8334,
    /// <summary>The object name has bad syntax.</summary>
    ERROR_DS_BAD_NAME_SYNTAX = 8335,
    /// <summary>It is not permitted for an alias to refer to another alias.</summary>
    ERROR_DS_ALIAS_POINTS_TO_ALIAS = 8336,
    /// <summary>The alias cannot be dereferenced.</summary>
    ERROR_DS_CANT_DEREF_ALIAS = 8337,
    /// <summary>The operation is out of scope.</summary>
    ERROR_DS_OUT_OF_SCOPE = 8338,
    /// <summary>The operation cannot continue because the object is in the process of being removed.</summary>
    ERROR_DS_OBJECT_BEING_REMOVED = 8339,
    /// <summary>The DSA object cannot be deleted.</summary>
    ERROR_DS_CANT_DELETE_DSA_OBJ = 8340,
    /// <summary>A directory service error has occurred.</summary>
    ERROR_DS_GENERIC_ERROR = 8341,
    /// <summary>The operation can only be performed on an internal master DSA object.</summary>
    ERROR_DS_DSA_MUST_BE_INT_MASTER = 8342,
    /// <summary>The object must be of class DSA.</summary>
    ERROR_DS_CLASS_NOT_DSA = 8343,
    /// <summary>Insufficient access rights to perform the operation.</summary>
    ERROR_DS_INSUFF_ACCESS_RIGHTS = 8344,
    /// <summary>The object cannot be added because the parent is not on the list of possible superiors.</summary>
    ERROR_DS_ILLEGAL_SUPERIOR = 8345,
    /// <summary>Access to the attribute is not permitted because the attribute is owned by the Security Accounts Manager (SAM).</summary>
    ERROR_DS_ATTRIBUTE_OWNED_BY_SAM = 8346,
    /// <summary>The name has too many parts.</summary>
    ERROR_DS_NAME_TOO_MANY_PARTS = 8347,
    /// <summary>The name is too long.</summary>
    ERROR_DS_NAME_TOO_LONG = 8348,
    /// <summary>The name value is too long.</summary>
    ERROR_DS_NAME_VALUE_TOO_LONG = 8349,
    /// <summary>The directory service encountered an error parsing a name.</summary>
    ERROR_DS_NAME_UNPARSEABLE = 8350,
    /// <summary>The directory service cannot get the attribute type for a name.</summary>
    ERROR_DS_NAME_TYPE_UNKNOWN = 8351,
    /// <summary>The name does not identify an object; the name identifies a phantom.</summary>
    ERROR_DS_NOT_AN_OBJECT = 8352,
    /// <summary>The security descriptor is too short.</summary>
    ERROR_DS_SEC_DESC_TOO_SHORT = 8353,
    /// <summary>The security descriptor is invalid.</summary>
    ERROR_DS_SEC_DESC_INVALID = 8354,
    /// <summary>Failed to create name for deleted object.</summary>
    ERROR_DS_NO_DELETED_NAME = 8355,
    /// <summary>The parent of a new subref must exist.</summary>
    ERROR_DS_SUBREF_MUST_HAVE_PARENT = 8356,
    /// <summary>The object must be a naming context.</summary>
    ERROR_DS_NCNAME_MUST_BE_NC = 8357,
    /// <summary>It is not permitted to add an attribute which is owned by the system.</summary>
    ERROR_DS_CANT_ADD_SYSTEM_ONLY = 8358,
    /// <summary>The class of the object must be structural; you cannot instantiate an abstract class.</summary>
    ERROR_DS_CLASS_MUST_BE_CONCRETE = 8359,
    /// <summary>The schema object could not be found.</summary>
    ERROR_DS_INVALID_DMD = 8360,
    /// <summary>A local object with this GUID (dead or alive) already exists.</summary>
    ERROR_DS_OBJ_GUID_EXISTS = 8361,
    /// <summary>The operation cannot be performed on a back link.</summary>
    ERROR_DS_NOT_ON_BACKLINK = 8362,
    /// <summary>The cross reference for the specified naming context could not be found.</summary>
    ERROR_DS_NO_CROSSREF_FOR_NC = 8363,
    /// <summary>The operation could not be performed because the directory service is shutting down.</summary>
    ERROR_DS_SHUTTING_DOWN = 8364,
    /// <summary>The directory service request is invalid.</summary>
    ERROR_DS_UNKNOWN_OPERATION = 8365,
    /// <summary>The role owner attribute could not be read.</summary>
    ERROR_DS_INVALID_ROLE_OWNER = 8366,
    /// <summary>The requested FSMO operation failed. The current FSMO holder could not be contacted.</summary>
    ERROR_DS_COULDNT_CONTACT_FSMO = 8367,
    /// <summary>Modification of a DN across a naming context is not permitted.</summary>
    ERROR_DS_CROSS_NC_DN_RENAME = 8368,
    /// <summary>The attribute cannot be modified because it is owned by the system.</summary>
    ERROR_DS_CANT_MOD_SYSTEM_ONLY = 8369,
    /// <summary>Only the replicator can perform this function.</summary>
    ERROR_DS_REPLICATOR_ONLY = 8370,
    /// <summary>The specified class is not defined.</summary>
    ERROR_DS_OBJ_CLASS_NOT_DEFINED = 8371,
    /// <summary>The specified class is not a subclass.</summary>
    ERROR_DS_OBJ_CLASS_NOT_SUBCLASS = 8372,
    /// <summary>The name reference is invalid.</summary>
    ERROR_DS_NAME_REFERENCE_INVALID = 8373,
    /// <summary>A cross reference already exists.</summary>
    ERROR_DS_CROSS_REF_EXISTS = 8374,
    /// <summary>It is not permitted to delete a master cross reference.</summary>
    ERROR_DS_CANT_DEL_MASTER_CROSSREF = 8375,
    /// <summary>Subtree notifications are only supported on NC heads.</summary>
    ERROR_DS_SUBTREE_NOTIFY_NOT_NC_HEAD = 8376,
    /// <summary>Notification filter is too complex.</summary>
    ERROR_DS_NOTIFY_FILTER_TOO_COMPLEX = 8377,
    /// <summary>Schema update failed: duplicate RDN.</summary>
    ERROR_DS_DUP_RDN = 8378,
    /// <summary>Schema update failed: duplicate OID.</summary>
    ERROR_DS_DUP_OID = 8379,
    /// <summary>Schema update failed: duplicate MAPI identifier.</summary>
    ERROR_DS_DUP_MAPI_ID = 8380,
    /// <summary>Schema update failed: duplicate schema-id GUID.</summary>
    ERROR_DS_DUP_SCHEMA_ID_GUID = 8381,
    /// <summary>Schema update failed: duplicate LDAP display name.</summary>
    ERROR_DS_DUP_LDAP_DISPLAY_NAME = 8382,
    /// <summary>Schema update failed: range-lower less than range upper.</summary>
    ERROR_DS_SEMANTIC_ATT_TEST = 8383,
    /// <summary>Schema update failed: syntax mismatch.</summary>
    ERROR_DS_SYNTAX_MISMATCH = 8384,
    /// <summary>Schema deletion failed: attribute is used in must-contain.</summary>
    ERROR_DS_EXISTS_IN_MUST_HAVE = 8385,
    /// <summary>Schema deletion failed: attribute is used in may-contain.</summary>
    ERROR_DS_EXISTS_IN_MAY_HAVE = 8386,
    /// <summary>Schema update failed: attribute in may-contain does not exist.</summary>
    ERROR_DS_NONEXISTENT_MAY_HAVE = 8387,
    /// <summary>Schema update failed: attribute in must-contain does not exist.</summary>
    ERROR_DS_NONEXISTENT_MUST_HAVE = 8388,
    /// <summary>Schema update failed: class in aux-class list does not exist or is not an auxiliary class.</summary>
    ERROR_DS_AUX_CLS_TEST_FAIL = 8389,
    /// <summary>Schema update failed: class in poss-superiors does not exist.</summary>
    ERROR_DS_NONEXISTENT_POSS_SUP = 8390,
    /// <summary>Schema update failed: class in subclassof list does not exist or does not satisfy hierarchy rules.</summary>
    ERROR_DS_SUB_CLS_TEST_FAIL = 8391,
    /// <summary>Schema update failed: Rdn-Att-Id has wrong syntax.</summary>
    ERROR_DS_BAD_RDN_ATT_ID_SYNTAX = 8392,
    /// <summary>Schema deletion failed: class is used as auxiliary class.</summary>
    ERROR_DS_EXISTS_IN_AUX_CLS = 8393,
    /// <summary>Schema deletion failed: class is used as sub class.</summary>
    ERROR_DS_EXISTS_IN_SUB_CLS = 8394,
    /// <summary>Schema deletion failed: class is used as poss superior.</summary>
    ERROR_DS_EXISTS_IN_POSS_SUP = 8395,
    /// <summary>Schema update failed in recalculating validation cache.</summary>
    ERROR_DS_RECALCSCHEMA_FAILED = 8396,
    /// <summary>The tree deletion is not finished. The request must be made again to continue deleting the tree.</summary>
    ERROR_DS_TREE_DELETE_NOT_FINISHED = 8397,
    /// <summary>The requested delete operation could not be performed.</summary>
    ERROR_DS_CANT_DELETE = 8398,
    /// <summary>Cannot read the governs class identifier for the schema record.</summary>
    ERROR_DS_ATT_SCHEMA_REQ_ID = 8399,
    /// <summary>The attribute schema has bad syntax.</summary>
    ERROR_DS_BAD_ATT_SCHEMA_SYNTAX = 8400,
    /// <summary>The attribute could not be cached.</summary>
    ERROR_DS_CANT_CACHE_ATT = 8401,
    /// <summary>The class could not be cached.</summary>
    ERROR_DS_CANT_CACHE_CLASS = 8402,
    /// <summary>The attribute could not be removed from the cache.</summary>
    ERROR_DS_CANT_REMOVE_ATT_CACHE = 8403,
    /// <summary>The class could not be removed from the cache.</summary>
    ERROR_DS_CANT_REMOVE_CLASS_CACHE = 8404,
    /// <summary>The distinguished name attribute could not be read.</summary>
    ERROR_DS_CANT_RETRIEVE_DN = 8405,
    /// <summary>No superior reference has been configured for the directory service. The directory service is therefore unable to issue referrals to objects outside this forest.</summary>
    ERROR_DS_MISSING_SUPREF = 8406,
    /// <summary>The instance type attribute could not be retrieved.</summary>
    ERROR_DS_CANT_RETRIEVE_INSTANCE = 8407,
    /// <summary>An internal error has occurred.</summary>
    ERROR_DS_CODE_INCONSISTENCY = 8408,
    /// <summary>A database error has occurred.</summary>
    ERROR_DS_DATABASE_ERROR = 8409,
    /// <summary>The attribute GOVERNSID is missing.</summary>
    ERROR_DS_GOVERNSID_MISSING = 8410,
    /// <summary>An expected attribute is missing.</summary>
    ERROR_DS_MISSING_EXPECTED_ATT = 8411,
    /// <summary>The specified naming context is missing a cross reference.</summary>
    ERROR_DS_NCNAME_MISSING_CR_REF = 8412,
    /// <summary>A security checking error has occurred.</summary>
    ERROR_DS_SECURITY_CHECKING_ERROR = 8413,
    /// <summary>The schema is not loaded.</summary>
    ERROR_DS_SCHEMA_NOT_LOADED = 8414,
    /// <summary>Schema allocation failed. Please check if the machine is running low on memory.</summary>
    ERROR_DS_SCHEMA_ALLOC_FAILED = 8415,
    /// <summary>Failed to obtain the required syntax for the attribute schema.</summary>
    ERROR_DS_ATT_SCHEMA_REQ_SYNTAX = 8416,
    /// <summary>The global catalog verification failed. The global catalog is not available or does not support the operation. Some part of the directory is currently not available.</summary>
    ERROR_DS_GCVERIFY_ERROR = 8417,
    /// <summary>The replication operation failed because of a schema mismatch between the servers involved.</summary>
    ERROR_DS_DRA_SCHEMA_MISMATCH = 8418,
    /// <summary>The DSA object could not be found.</summary>
    ERROR_DS_CANT_FIND_DSA_OBJ = 8419,
    /// <summary>The naming context could not be found.</summary>
    ERROR_DS_CANT_FIND_EXPECTED_NC = 8420,
    /// <summary>The naming context could not be found in the cache.</summary>
    ERROR_DS_CANT_FIND_NC_IN_CACHE = 8421,
    /// <summary>The child object could not be retrieved.</summary>
    ERROR_DS_CANT_RETRIEVE_CHILD = 8422,
    /// <summary>The modification was not permitted for security reasons.</summary>
    ERROR_DS_SECURITY_ILLEGAL_MODIFY = 8423,
    /// <summary>The operation cannot replace the hidden record.</summary>
    ERROR_DS_CANT_REPLACE_HIDDEN_REC = 8424,
    /// <summary>The hierarchy file is invalid.</summary>
    ERROR_DS_BAD_HIERARCHY_FILE = 8425,
    /// <summary>The attempt to build the hierarchy table failed.</summary>
    ERROR_DS_BUILD_HIERARCHY_TABLE_FAILED = 8426,
    /// <summary>The directory configuration parameter is missing from the registry.</summary>
    ERROR_DS_CONFIG_PARAM_MISSING = 8427,
    /// <summary>The attempt to count the address book indices failed.</summary>
    ERROR_DS_COUNTING_AB_INDICES_FAILED = 8428,
    /// <summary>The allocation of the hierarchy table failed.</summary>
    ERROR_DS_HIERARCHY_TABLE_MALLOC_FAILED = 8429,
    /// <summary>The directory service encountered an internal failure.</summary>
    ERROR_DS_INTERNAL_FAILURE = 8430,
    /// <summary>The directory service encountered an unknown failure.</summary>
    ERROR_DS_UNKNOWN_ERROR = 8431,
    /// <summary>A root object requires a class of &#39;top&#39;.</summary>
    ERROR_DS_ROOT_REQUIRES_CLASS_TOP = 8432,
    /// <summary>This directory server is shutting down, and cannot take ownership of new floating single-master operation roles.</summary>
    ERROR_DS_REFUSING_FSMO_ROLES = 8433,
    /// <summary>The directory service is missing mandatory configuration information, and is unable to determine the ownership of floating single-master operation roles.</summary>
    ERROR_DS_MISSING_FSMO_SETTINGS = 8434,
    /// <summary>The directory service was unable to transfer ownership of one or more floating single-master operation roles to other servers.</summary>
    ERROR_DS_UNABLE_TO_SURRENDER_ROLES = 8435,
    /// <summary>The replication operation failed.</summary>
    ERROR_DS_DRA_GENERIC = 8436,
    /// <summary>An invalid parameter was specified for this replication operation.</summary>
    ERROR_DS_DRA_INVALID_PARAMETER = 8437,
    /// <summary>The directory service is too busy to complete the replication operation at this time.</summary>
    ERROR_DS_DRA_BUSY = 8438,
    /// <summary>The distinguished name specified for this replication operation is invalid.</summary>
    ERROR_DS_DRA_BAD_DN = 8439,
    /// <summary>The naming context specified for this replication operation is invalid.</summary>
    ERROR_DS_DRA_BAD_NC = 8440,
    /// <summary>The distinguished name specified for this replication operation already exists.</summary>
    ERROR_DS_DRA_DN_EXISTS = 8441,
    /// <summary>The replication system encountered an internal error.</summary>
    ERROR_DS_DRA_INTERNAL_ERROR = 8442,
    /// <summary>The replication operation encountered a database inconsistency.</summary>
    ERROR_DS_DRA_INCONSISTENT_DIT = 8443,
    /// <summary>The server specified for this replication operation could not be contacted.</summary>
    ERROR_DS_DRA_CONNECTION_FAILED = 8444,
    /// <summary>The replication operation encountered an object with an invalid instance type.</summary>
    ERROR_DS_DRA_BAD_INSTANCE_TYPE = 8445,
    /// <summary>The replication operation failed to allocate memory.</summary>
    ERROR_DS_DRA_OUT_OF_MEM = 8446,
    /// <summary>The replication operation encountered an error with the mail system.</summary>
    ERROR_DS_DRA_MAIL_PROBLEM = 8447,
    /// <summary>The replication reference information for the target server already exists.</summary>
    ERROR_DS_DRA_REF_ALREADY_EXISTS = 8448,
    /// <summary>The replication reference information for the target server does not exist.</summary>
    ERROR_DS_DRA_REF_NOT_FOUND = 8449,
    /// <summary>The naming context cannot be removed because it is replicated to another server.</summary>
    ERROR_DS_DRA_OBJ_IS_REP_SOURCE = 8450,
    /// <summary>The replication operation encountered a database error.</summary>
    ERROR_DS_DRA_DB_ERROR = 8451,
    /// <summary>The naming context is in the process of being removed or is not replicated from the specified server.</summary>
    ERROR_DS_DRA_NO_REPLICA = 8452,
    /// <summary>Replication access was denied.</summary>
    ERROR_DS_DRA_ACCESS_DENIED = 8453,
    /// <summary>The requested operation is not supported by this version of the directory service.</summary>
    ERROR_DS_DRA_NOT_SUPPORTED = 8454,
    /// <summary>The replication remote procedure call was cancelled.</summary>
    ERROR_DS_DRA_RPC_CANCELLED = 8455,
    /// <summary>The source server is currently rejecting replication requests.</summary>
    ERROR_DS_DRA_SOURCE_DISABLED = 8456,
    /// <summary>The destination server is currently rejecting replication requests.</summary>
    ERROR_DS_DRA_SINK_DISABLED = 8457,
    /// <summary>The replication operation failed due to a collision of object names.</summary>
    ERROR_DS_DRA_NAME_COLLISION = 8458,
    /// <summary>The replication source has been reinstalled.</summary>
    ERROR_DS_DRA_SOURCE_REINSTALLED = 8459,
    /// <summary>The replication operation failed because a required parent object is missing.</summary>
    ERROR_DS_DRA_MISSING_PARENT = 8460,
    /// <summary>The replication operation was preempted.</summary>
    ERROR_DS_DRA_PREEMPTED = 8461,
    /// <summary>The replication synchronization attempt was abandoned because of a lack of updates.</summary>
    ERROR_DS_DRA_ABANDON_SYNC = 8462,
    /// <summary>The replication operation was terminated because the system is shutting down.</summary>
    ERROR_DS_DRA_SHUTDOWN = 8463,
    /// <summary>Synchronization attempt failed because the destination DC is currently waiting to synchronize new partial attributes from source. This condition is normal if a recent schema change modified the partial attribute set. The destination partial attribute set is not a subset of source partial attribute set.</summary>
    ERROR_DS_DRA_INCOMPATIBLE_PARTIAL_SET = 8464,
    /// <summary>The replication synchronization attempt failed because a master replica attempted to sync from a partial replica.</summary>
    ERROR_DS_DRA_SOURCE_IS_PARTIAL_REPLICA = 8465,
    /// <summary>The server specified for this replication operation was contacted, but that server was unable to contact an additional server needed to complete the operation.</summary>
    ERROR_DS_DRA_EXTN_CONNECTION_FAILED = 8466,
    /// <summary>The version of the directory service schema of the source forest is not compatible with the version of directory service on this computer.</summary>
    ERROR_DS_INSTALL_SCHEMA_MISMATCH = 8467,
    /// <summary>Schema update failed: An attribute with the same link identifier already exists.</summary>
    ERROR_DS_DUP_LINK_ID = 8468,
    /// <summary>Name translation: Generic processing error.</summary>
    ERROR_DS_NAME_ERROR_RESOLVING = 8469,
    /// <summary>Name translation: Could not find the name or insufficient right to see name.</summary>
    ERROR_DS_NAME_ERROR_NOT_FOUND = 8470,
    /// <summary>Name translation: Input name mapped to more than one output name.</summary>
    ERROR_DS_NAME_ERROR_NOT_UNIQUE = 8471,
    /// <summary>Name translation: Input name found, but not the associated output format.</summary>
    ERROR_DS_NAME_ERROR_NO_MAPPING = 8472,
    /// <summary>Name translation: Unable to resolve completely, only the domain was found.</summary>
    ERROR_DS_NAME_ERROR_DOMAIN_ONLY = 8473,
    /// <summary>Name translation: Unable to perform purely syntactical mapping at the client without going out to the wire.</summary>
    ERROR_DS_NAME_ERROR_NO_SYNTACTICAL_MAPPING = 8474,
    /// <summary>Modification of a constructed attribute is not allowed.</summary>
    ERROR_DS_CONSTRUCTED_ATT_MOD = 8475,
    /// <summary>The OM-Object-Class specified is incorrect for an attribute with the specified syntax.</summary>
    ERROR_DS_WRONG_OM_OBJ_CLASS = 8476,
    /// <summary>The replication request has been posted; waiting for reply.</summary>
    ERROR_DS_DRA_REPL_PENDING = 8477,
    /// <summary>The requested operation requires a directory service, and none was available.</summary>
    ERROR_DS_DS_REQUIRED = 8478,
    /// <summary>The LDAP display name of the class or attribute contains non-ASCII characters.</summary>
    ERROR_DS_INVALID_LDAP_DISPLAY_NAME = 8479,
    /// <summary>The requested search operation is only supported for base searches.</summary>
    ERROR_DS_NON_BASE_SEARCH = 8480,
    /// <summary>The search failed to retrieve attributes from the database.</summary>
    ERROR_DS_CANT_RETRIEVE_ATTS = 8481,
    /// <summary>The schema update operation tried to add a backward link attribute that has no corresponding forward link.</summary>
    ERROR_DS_BACKLINK_WITHOUT_LINK = 8482,
    /// <summary>Source and destination of a cross-domain move do not agree on the object&#39;s epoch number. Either source or destination does not have the latest version of the object.</summary>
    ERROR_DS_EPOCH_MISMATCH = 8483,
    /// <summary>Source and destination of a cross-domain move do not agree on the object&#39;s current name. Either source or destination does not have the latest version of the object.</summary>
    ERROR_DS_SRC_NAME_MISMATCH = 8484,
    /// <summary>Source and destination for the cross-domain move operation are identical. Caller should use local move operation instead of cross-domain move operation.</summary>
    ERROR_DS_SRC_AND_DST_NC_IDENTICAL = 8485,
    /// <summary>Source and destination for a cross-domain move are not in agreement on the naming contexts in the forest. Either source or destination does not have the latest version of the Partitions container.</summary>
    ERROR_DS_DST_NC_MISMATCH = 8486,
    /// <summary>Destination of a cross-domain move is not authoritative for the destination naming context.</summary>
    ERROR_DS_NOT_AUTHORITIVE_FOR_DST_NC = 8487,
    /// <summary>Source and destination of a cross-domain move do not agree on the identity of the source object. Either source or destination does not have the latest version of the source object.</summary>
    ERROR_DS_SRC_GUID_MISMATCH = 8488,
    /// <summary>Object being moved across-domains is already known to be deleted by the destination server. The source server does not have the latest version of the source object.</summary>
    ERROR_DS_CANT_MOVE_DELETED_OBJECT = 8489,
    /// <summary>Another operation which requires exclusive access to the PDC FSMO is already in progress.</summary>
    ERROR_DS_PDC_OPERATION_IN_PROGRESS = 8490,
    /// <summary>A cross-domain move operation failed such that two versions of the moved object exist - one each in the source and destination domains. The destination object needs to be removed to restore the system to a consistent state.</summary>
    ERROR_DS_CROSS_DOMAIN_CLEANUP_REQD = 8491,
    /// <summary>This object may not be moved across domain boundaries either because cross-domain moves for this class are disallowed, or the object has some special characteristics, e.g.: trust account or restricted RID, which prevent its move.</summary>
    ERROR_DS_ILLEGAL_XDOM_MOVE_OPERATION = 8492,
    /// <summary>Can&#39;t move objects with memberships across domain boundaries as once moved, this would violate the membership conditions of the account group. Remove the object from any account group memberships and retry.</summary>
    ERROR_DS_CANT_WITH_ACCT_GROUP_MEMBERSHPS = 8493,
    /// <summary>A naming context head must be the immediate child of another naming context head, not of an interior node.</summary>
    ERROR_DS_NC_MUST_HAVE_NC_PARENT = 8494,
    /// <summary>The directory cannot validate the proposed naming context name because it does not hold a replica of the naming context above the proposed naming context. Please ensure that the domain naming master role is held by a server that is configured as a global catalog server, and that the server is up to date with its replication partners. (Applies only to Windows 2000 Domain Naming masters)</summary>
    ERROR_DS_CR_IMPOSSIBLE_TO_VALIDATE = 8495,
    /// <summary>Destination domain must be in native mode.</summary>
    ERROR_DS_DST_DOMAIN_NOT_NATIVE = 8496,
    /// <summary>The operation cannot be performed because the server does not have an infrastructure container in the domain of interest.</summary>
    ERROR_DS_MISSING_INFRASTRUCTURE_CONTAINER = 8497,
    /// <summary>Cross-domain move of non-empty account groups is not allowed.</summary>
    ERROR_DS_CANT_MOVE_ACCOUNT_GROUP = 8498,
    /// <summary>Cross-domain move of non-empty resource groups is not allowed.</summary>
    ERROR_DS_CANT_MOVE_RESOURCE_GROUP = 8499,
    /// <summary>The search flags for the attribute are invalid. The ANR bit is valid only on attributes of Unicode or Teletex strings.</summary>
    ERROR_DS_INVALID_SEARCH_FLAG = 8500,
    /// <summary>Tree deletions starting at an object which has an NC head as a descendant are not allowed.</summary>
    ERROR_DS_NO_TREE_DELETE_ABOVE_NC = 8501,
    /// <summary>The directory service failed to lock a tree in preparation for a tree deletion because the tree was in use.</summary>
    ERROR_DS_COULDNT_LOCK_TREE_FOR_DELETE = 8502,
    /// <summary>The directory service failed to identify the list of objects to delete while attempting a tree deletion.</summary>
    ERROR_DS_COULDNT_IDENTIFY_OBJECTS_FOR_TREE_DELETE = 8503,
    /// <summary>Security Accounts Manager initialization failed because of the following error: %1. Error Status: 0x%2. Please shutdown this system and reboot into Directory Services Restore Mode, check the event log for more detailed information.</summary>
    ERROR_DS_SAM_INIT_FAILURE = 8504,
    /// <summary>Only an administrator can modify the membership list of an administrative group.</summary>
    ERROR_DS_SENSITIVE_GROUP_VIOLATION = 8505,
    /// <summary>Cannot change the primary group ID of a domain controller account.</summary>
    ERROR_DS_CANT_MOD_PRIMARYGROUPID = 8506,
    /// <summary>An attempt is made to modify the base schema.</summary>
    ERROR_DS_ILLEGAL_BASE_SCHEMA_MOD = 8507,
    /// <summary>Adding a new mandatory attribute to an existing class, deleting a mandatory attribute from an existing class, or adding an optional attribute to the special class Top that is not a backlink attribute (directly or through inheritance, for example, by adding or deleting an auxiliary class) is not allowed.</summary>
    ERROR_DS_NONSAFE_SCHEMA_CHANGE = 8508,
    /// <summary>Schema update is not allowed on this DC because the DC is not the schema FSMO Role Owner.</summary>
    ERROR_DS_SCHEMA_UPDATE_DISALLOWED = 8509,
    /// <summary>An object of this class cannot be created under the schema container. You can only create attribute-schema and class-schema objects under the schema container.</summary>
    ERROR_DS_CANT_CREATE_UNDER_SCHEMA = 8510,
    /// <summary>The replica/child install failed to get the objectVersion attribute on the schema container on the source DC. Either the attribute is missing on the schema container or the credentials supplied do not have permission to read it.</summary>
    ERROR_DS_INSTALL_NO_SRC_SCH_VERSION = 8511,
    /// <summary>The replica/child install failed to read the objectVersion attribute in the SCHEMA section of the file schema.ini in the system32 directory.</summary>
    ERROR_DS_INSTALL_NO_SCH_VERSION_IN_INIFILE = 8512,
    /// <summary>The specified group type is invalid.</summary>
    ERROR_DS_INVALID_GROUP_TYPE = 8513,
    /// <summary>You cannot nest global groups in a mixed domain if the group is security-enabled.</summary>
    ERROR_DS_NO_NEST_GLOBALGROUP_IN_MIXEDDOMAIN = 8514,
    /// <summary>You cannot nest local groups in a mixed domain if the group is security-enabled.</summary>
    ERROR_DS_NO_NEST_LOCALGROUP_IN_MIXEDDOMAIN = 8515,
    /// <summary>A global group cannot have a local group as a member.</summary>
    ERROR_DS_GLOBAL_CANT_HAVE_LOCAL_MEMBER = 8516,
    /// <summary>A global group cannot have a universal group as a member.</summary>
    ERROR_DS_GLOBAL_CANT_HAVE_UNIVERSAL_MEMBER = 8517,
    /// <summary>A universal group cannot have a local group as a member.</summary>
    ERROR_DS_UNIVERSAL_CANT_HAVE_LOCAL_MEMBER = 8518,
    /// <summary>A global group cannot have a cross-domain member.</summary>
    ERROR_DS_GLOBAL_CANT_HAVE_CROSSDOMAIN_MEMBER = 8519,
    /// <summary>A local group cannot have another cross domain local group as a member.</summary>
    ERROR_DS_LOCAL_CANT_HAVE_CROSSDOMAIN_LOCAL_MEMBER = 8520,
    /// <summary>A group with primary members cannot change to a security-disabled group.</summary>
    ERROR_DS_HAVE_PRIMARY_MEMBERS = 8521,
    /// <summary>The schema cache load failed to convert the string default SD on a class-schema object.</summary>
    ERROR_DS_STRING_SD_CONVERSION_FAILED = 8522,
    /// <summary>Only DSAs configured to be Global Catalog servers should be allowed to hold the Domain Naming Master FSMO role. (Applies only to Windows 2000 servers)</summary>
    ERROR_DS_NAMING_MASTER_GC = 8523,
    /// <summary>The DSA operation is unable to proceed because of a DNS lookup failure.</summary>
    ERROR_DS_DNS_LOOKUP_FAILURE = 8524,
    /// <summary>While processing a change to the DNS Host Name for an object, the Service Principal Name values could not be kept in sync.</summary>
    ERROR_DS_COULDNT_UPDATE_SPNS = 8525,
    /// <summary>The Security Descriptor attribute could not be read.</summary>
    ERROR_DS_CANT_RETRIEVE_SD = 8526,
    /// <summary>The object requested was not found, but an object with that key was found.</summary>
    ERROR_DS_KEY_NOT_UNIQUE = 8527,
    /// <summary>The syntax of the linked attribute being added is incorrect. Forward links can only have syntax 2.5.5.1, 2.5.5.7, and 2.5.5.14, and backlinks can only have syntax 2.5.5.1</summary>
    ERROR_DS_WRONG_LINKED_ATT_SYNTAX = 8528,
    /// <summary>Security Account Manager needs to get the boot password.</summary>
    ERROR_DS_SAM_NEED_BOOTKEY_PASSWORD = 8529,
    /// <summary>Security Account Manager needs to get the boot key from floppy disk.</summary>
    ERROR_DS_SAM_NEED_BOOTKEY_FLOPPY = 8530,
    /// <summary>Directory Service cannot start.</summary>
    ERROR_DS_CANT_START = 8531,
    /// <summary>Directory Services could not start.</summary>
    ERROR_DS_INIT_FAILURE = 8532,
    /// <summary>The connection between client and server requires packet privacy or better.</summary>
    ERROR_DS_NO_PKT_PRIVACY_ON_CONNECTION = 8533,
    /// <summary>The source domain may not be in the same forest as destination.</summary>
    ERROR_DS_SOURCE_DOMAIN_IN_FOREST = 8534,
    /// <summary>The destination domain must be in the forest.</summary>
    ERROR_DS_DESTINATION_DOMAIN_NOT_IN_FOREST = 8535,
    /// <summary>The operation requires that destination domain auditing be enabled.</summary>
    ERROR_DS_DESTINATION_AUDITING_NOT_ENABLED = 8536,
    /// <summary>The operation couldn&#39;t locate a DC for the source domain.</summary>
    ERROR_DS_CANT_FIND_DC_FOR_SRC_DOMAIN = 8537,
    /// <summary>The source object must be a group or user.</summary>
    ERROR_DS_SRC_OBJ_NOT_GROUP_OR_USER = 8538,
    /// <summary>The source object&#39;s SID already exists in destination forest.</summary>
    ERROR_DS_SRC_SID_EXISTS_IN_FOREST = 8539,
    /// <summary>The source and destination object must be of the same type.</summary>
    ERROR_DS_SRC_AND_DST_OBJECT_CLASS_MISMATCH = 8540,
    /// <summary>Security Accounts Manager initialization failed because of the following error: %1. Error Status: 0x%2. Click OK to shut down the system and reboot into Safe Mode. Check the event log for detailed information.</summary>
    ERROR_SAM_INIT_FAILURE = 8541,
    /// <summary>Schema information could not be included in the replication request.</summary>
    ERROR_DS_DRA_SCHEMA_INFO_SHIP = 8542,
    /// <summary>The replication operation could not be completed due to a schema incompatibility.</summary>
    ERROR_DS_DRA_SCHEMA_CONFLICT = 8543,
    /// <summary>The replication operation could not be completed due to a previous schema incompatibility.</summary>
    ERROR_DS_DRA_EARLIER_SCHEMA_CONFLICT = 8544,
    /// <summary>The replication update could not be applied because either the source or the destination has not yet received information regarding a recent cross-domain move operation.</summary>
    ERROR_DS_DRA_OBJ_NC_MISMATCH = 8545,
    /// <summary>The requested domain could not be deleted because there exist domain controllers that still host this domain.</summary>
    ERROR_DS_NC_STILL_HAS_DSAS = 8546,
    /// <summary>The requested operation can be performed only on a global catalog server.</summary>
    ERROR_DS_GC_REQUIRED = 8547,
    /// <summary>A local group can only be a member of other local groups in the same domain.</summary>
    ERROR_DS_LOCAL_MEMBER_OF_LOCAL_ONLY = 8548,
    /// <summary>Foreign security principals cannot be members of universal groups.</summary>
    ERROR_DS_NO_FPO_IN_UNIVERSAL_GROUPS = 8549,
    /// <summary>The attribute is not allowed to be replicated to the GC because of security reasons.</summary>
    ERROR_DS_CANT_ADD_TO_GC = 8550,
    /// <summary>The checkpoint with the PDC could not be taken because there too many modifications being processed currently.</summary>
    ERROR_DS_NO_CHECKPOINT_WITH_PDC = 8551,
    /// <summary>The operation requires that source domain auditing be enabled.</summary>
    ERROR_DS_SOURCE_AUDITING_NOT_ENABLED = 8552,
    /// <summary>Security principal objects can only be created inside domain naming contexts.</summary>
    ERROR_DS_CANT_CREATE_IN_NONDOMAIN_NC = 8553,
    /// <summary>A Service Principal Name (SPN) could not be constructed because the provided hostname is not in the necessary format.</summary>
    ERROR_DS_INVALID_NAME_FOR_SPN = 8554,
    /// <summary>A Filter was passed that uses constructed attributes.</summary>
    ERROR_DS_FILTER_USES_CONTRUCTED_ATTRS = 8555,
    /// <summary>The unicodePwd attribute value must be enclosed in double quotes.</summary>
    ERROR_DS_UNICODEPWD_NOT_IN_QUOTES = 8556,
    /// <summary>Your computer could not be joined to the domain. You have exceeded the maximum number of computer accounts you are allowed to create in this domain. Contact your system administrator to have this limit reset or increased.</summary>
    ERROR_DS_MACHINE_ACCOUNT_QUOTA_EXCEEDED = 8557,
    /// <summary>For security reasons, the operation must be run on the destination DC.</summary>
    ERROR_DS_MUST_BE_RUN_ON_DST_DC = 8558,
    /// <summary>For security reasons, the source DC must be NT4SP4 or greater.</summary>
    ERROR_DS_SRC_DC_MUST_BE_SP4_OR_GREATER = 8559,
    /// <summary>Critical Directory Service System objects cannot be deleted during tree delete operations. The tree delete may have been partially performed.</summary>
    ERROR_DS_CANT_TREE_DELETE_CRITICAL_OBJ = 8560,
    /// <summary>Directory Services could not start because of the following error: %1. Error Status: 0x%2. Please click OK to shutdown the system. You can use the recovery console to diagnose the system further.</summary>
    ERROR_DS_INIT_FAILURE_CONSOLE = 8561,
    /// <summary>Security Accounts Manager initialization failed because of the following error: %1. Error Status: 0x%2. Please click OK to shutdown the system. You can use the recovery console to diagnose the system further.</summary>
    ERROR_DS_SAM_INIT_FAILURE_CONSOLE = 8562,
    /// <summary>The version of the operating system is incompatible with the current AD DS forest functional level or AD LDS Configuration Set functional level. You must upgrade to a new version of the operating system before this server can become an AD DS Domain Controller or add an AD LDS Instance in this AD DS Forest or AD LDS Configuration Set.</summary>
    ERROR_DS_FOREST_VERSION_TOO_HIGH = 8563,
    /// <summary>The version of the operating system installed is incompatible with the current domain functional level. You must upgrade to a new version of the operating system before this server can become a domain controller in this domain.</summary>
    ERROR_DS_DOMAIN_VERSION_TOO_HIGH = 8564,
    /// <summary>The version of the operating system installed on this server no longer supports the current AD DS Forest functional level or AD LDS Configuration Set functional level. You must raise the AD DS Forest functional level or AD LDS Configuration Set functional level before this server can become an AD DS Domain Controller or an AD LDS Instance in this Forest or Configuration Set.</summary>
    ERROR_DS_FOREST_VERSION_TOO_LOW = 8565,
    /// <summary>The version of the operating system installed on this server no longer supports the current domain functional level. You must raise the domain functional level before this server can become a domain controller in this domain.</summary>
    ERROR_DS_DOMAIN_VERSION_TOO_LOW = 8566,
    /// <summary>The version of the operating system installed on this server is incompatible with the functional level of the domain or forest.</summary>
    ERROR_DS_INCOMPATIBLE_VERSION = 8567,
    /// <summary>The functional level of the domain (or forest) cannot be raised to the requested value, because there exist one or more domain controllers in the domain (or forest) that are at a lower incompatible functional level.</summary>
    ERROR_DS_LOW_DSA_VERSION = 8568,
    /// <summary>The forest functional level cannot be raised to the requested value since one or more domains are still in mixed domain mode. All domains in the forest must be in native mode, for you to raise the forest functional level.</summary>
    ERROR_DS_NO_BEHAVIOR_VERSION_IN_MIXEDDOMAIN = 8569,
    /// <summary>The sort order requested is not supported.</summary>
    ERROR_DS_NOT_SUPPORTED_SORT_ORDER = 8570,
    /// <summary>The requested name already exists as a unique identifier.</summary>
    ERROR_DS_NAME_NOT_UNIQUE = 8571,
    /// <summary>The machine account was created pre-NT4. The account needs to be recreated.</summary>
    ERROR_DS_MACHINE_ACCOUNT_CREATED_PRENT4 = 8572,
    /// <summary>The database is out of version store.</summary>
    ERROR_DS_OUT_OF_VERSION_STORE = 8573,
    /// <summary>Unable to continue operation because multiple conflicting controls were used.</summary>
    ERROR_DS_INCOMPATIBLE_CONTROLS_USED = 8574,
    /// <summary>Unable to find a valid security descriptor reference domain for this partition.</summary>
    ERROR_DS_NO_REF_DOMAIN = 8575,
    /// <summary>Schema update failed: The link identifier is reserved.</summary>
    ERROR_DS_RESERVED_LINK_ID = 8576,
    /// <summary>Schema update failed: There are no link identifiers available.</summary>
    ERROR_DS_LINK_ID_NOT_AVAILABLE = 8577,
    /// <summary>An account group cannot have a universal group as a member.</summary>
    ERROR_DS_AG_CANT_HAVE_UNIVERSAL_MEMBER = 8578,
    /// <summary>Rename or move operations on naming context heads or read-only objects are not allowed.</summary>
    ERROR_DS_MODIFYDN_DISALLOWED_BY_INSTANCE_TYPE = 8579,
    /// <summary>Move operations on objects in the schema naming context are not allowed.</summary>
    ERROR_DS_NO_OBJECT_MOVE_IN_SCHEMA_NC = 8580,
    /// <summary>A system flag has been set on the object and does not allow the object to be moved or renamed.</summary>
    ERROR_DS_MODIFYDN_DISALLOWED_BY_FLAG = 8581,
    /// <summary>This object is not allowed to change its grandparent container. Moves are not forbidden on this object, but are restricted to sibling containers.</summary>
    ERROR_DS_MODIFYDN_WRONG_GRANDPARENT = 8582,
    /// <summary>Unable to resolve completely, a referral to another forest is generated.</summary>
    ERROR_DS_NAME_ERROR_TRUST_REFERRAL = 8583,
    /// <summary>The requested action is not supported on standard server.</summary>
    ERROR_NOT_SUPPORTED_ON_STANDARD_SERVER = 8584,
    /// <summary>Could not access a partition of the directory service located on a remote server. Make sure at least one server is running for the partition in question.</summary>
    ERROR_DS_CANT_ACCESS_REMOTE_PART_OF_AD = 8585,
    /// <summary>The directory cannot validate the proposed naming context (or partition) name because it does not hold a replica nor can it contact a replica of the naming context above the proposed naming context. Please ensure that the parent naming context is properly registered in DNS, and at least one replica of this naming context is reachable by the Domain Naming master.</summary>
    ERROR_DS_CR_IMPOSSIBLE_TO_VALIDATE_V2 = 8586,
    /// <summary>The thread limit for this request was exceeded.</summary>
    ERROR_DS_THREAD_LIMIT_EXCEEDED = 8587,
    /// <summary>The Global catalog server is not in the closest site.</summary>
    ERROR_DS_NOT_CLOSEST = 8588,
    /// <summary>The DS cannot derive a service principal name (SPN) with which to mutually authenticate the target server because the corresponding server object in the local DS database has no serverReference attribute.</summary>
    ERROR_DS_CANT_DERIVE_SPN_WITHOUT_SERVER_REF = 8589,
    /// <summary>The Directory Service failed to enter single user mode.</summary>
    ERROR_DS_SINGLE_USER_MODE_FAILED = 8590,
    /// <summary>The Directory Service cannot parse the script because of a syntax error.</summary>
    ERROR_DS_NTDSCRIPT_SYNTAX_ERROR = 8591,
    /// <summary>The Directory Service cannot process the script because of an error.</summary>
    ERROR_DS_NTDSCRIPT_PROCESS_ERROR = 8592,
    /// <summary>The directory service cannot perform the requested operation because the servers involved are of different replication epochs (which is usually related to a domain rename that is in progress).</summary>
    ERROR_DS_DIFFERENT_REPL_EPOCHS = 8593,
    /// <summary>The directory service binding must be renegotiated due to a change in the server extensions information.</summary>
    ERROR_DS_DRS_EXTENSIONS_CHANGED = 8594,
    /// <summary>Operation not allowed on a disabled cross ref.</summary>
    ERROR_DS_REPLICA_SET_CHANGE_NOT_ALLOWED_ON_DISABLED_CR = 8595,
    /// <summary>Schema update failed: No values for msDS-IntId are available.</summary>
    ERROR_DS_NO_MSDS_INTID = 8596,
    /// <summary>Schema update failed: Duplicate msDS-INtId. Retry the operation.</summary>
    ERROR_DS_DUP_MSDS_INTID = 8597,
    /// <summary>Schema deletion failed: attribute is used in rDNAttID.</summary>
    ERROR_DS_EXISTS_IN_RDNATTID = 8598,
    /// <summary>The directory service failed to authorize the request.</summary>
    ERROR_DS_AUTHORIZATION_FAILED = 8599,
    /// <summary>The Directory Service cannot process the script because it is invalid.</summary>
    ERROR_DS_INVALID_SCRIPT = 8600,
    /// <summary>The remote create cross reference operation failed on the Domain Naming Master FSMO. The operation&#39;s error is in the extended data.</summary>
    ERROR_DS_REMOTE_CROSSREF_OP_FAILED = 8601,
    /// <summary>A cross reference is in use locally with the same name.</summary>
    ERROR_DS_CROSS_REF_BUSY = 8602,
    /// <summary>The DS cannot derive a service principal name (SPN) with which to mutually authenticate the target server because the server&#39;s domain has been deleted from the forest.</summary>
    ERROR_DS_CANT_DERIVE_SPN_FOR_DELETED_DOMAIN = 8603,
    /// <summary>Writeable NCs prevent this DC from demoting.</summary>
    ERROR_DS_CANT_DEMOTE_WITH_WRITEABLE_NC = 8604,
    /// <summary>The requested object has a non-unique identifier and cannot be retrieved.</summary>
    ERROR_DS_DUPLICATE_ID_FOUND = 8605,
    /// <summary>Insufficient attributes were given to create an object. This object may not exist because it may have been deleted and already garbage collected.</summary>
    ERROR_DS_INSUFFICIENT_ATTR_TO_CREATE_OBJECT = 8606,
    /// <summary>The group cannot be converted due to attribute restrictions on the requested group type.</summary>
    ERROR_DS_GROUP_CONVERSION_ERROR = 8607,
    /// <summary>Cross-domain move of non-empty basic application groups is not allowed.</summary>
    ERROR_DS_CANT_MOVE_APP_BASIC_GROUP = 8608,
    /// <summary>Cross-domain move of non-empty query based application groups is not allowed.</summary>
    ERROR_DS_CANT_MOVE_APP_QUERY_GROUP = 8609,
    /// <summary>The FSMO role ownership could not be verified because its directory partition has not replicated successfully with at least one replication partner.</summary>
    ERROR_DS_ROLE_NOT_VERIFIED = 8610,
    /// <summary>The target container for a redirection of a well known object container cannot already be a special container.</summary>
    ERROR_DS_WKO_CONTAINER_CANNOT_BE_SPECIAL = 8611,
    /// <summary>The Directory Service cannot perform the requested operation because a domain rename operation is in progress.</summary>
    ERROR_DS_DOMAIN_RENAME_IN_PROGRESS = 8612,
    /// <summary>The directory service detected a child partition below the requested partition name. The partition hierarchy must be created in a top down method.</summary>
    ERROR_DS_EXISTING_AD_CHILD_NC = 8613,
    /// <summary>The directory service cannot replicate with this server because the time since the last replication with this server has exceeded the tombstone lifetime.</summary>
    ERROR_DS_REPL_LIFETIME_EXCEEDED = 8614,
    /// <summary>The requested operation is not allowed on an object under the system container.</summary>
    ERROR_DS_DISALLOWED_IN_SYSTEM_CONTAINER = 8615,
    /// <summary>The LDAP servers network send queue has filled up because the client is not processing the results of it&#39;s requests fast enough. No more requests will be processed until the client catches up. If the client does not catch up then it will be disconnected.</summary>
    ERROR_DS_LDAP_SEND_QUEUE_FULL = 8616,
    /// <summary>The scheduled replication did not take place because the system was too busy to execute the request within the schedule window. The replication queue is overloaded. Consider reducing the number of partners or decreasing the scheduled replication frequency.</summary>
    ERROR_DS_DRA_OUT_SCHEDULE_WINDOW = 8617,
    /// <summary>At this time, it cannot be determined if the branch replication policy is available on the hub domain controller. Please retry at a later time to account for replication latencies.</summary>
    ERROR_DS_POLICY_NOT_KNOWN = 8618,
    /// <summary>The site settings object for the specified site does not exist.</summary>
    ERROR_NO_SITE_SETTINGS_OBJECT = 8619,
    /// <summary>The local account store does not contain secret material for the specified account.</summary>
    ERROR_NO_SECRETS = 8620,
    /// <summary>Could not find a writable domain controller in the domain.</summary>
    ERROR_NO_WRITABLE_DC_FOUND = 8621,
    /// <summary>The server object for the domain controller does not exist.</summary>
    ERROR_DS_NO_SERVER_OBJECT = 8622,
    /// <summary>The NTDS Settings object for the domain controller does not exist.</summary>
    ERROR_DS_NO_NTDSA_OBJECT = 8623,
    /// <summary>The requested search operation is not supported for ASQ searches.</summary>
    ERROR_DS_NON_ASQ_SEARCH = 8624,
    /// <summary>A required audit event could not be generated for the operation.</summary>
    ERROR_DS_AUDIT_FAILURE = 8625,
    /// <summary>The search flags for the attribute are invalid. The subtree index bit is valid only on single valued attributes.</summary>
    ERROR_DS_INVALID_SEARCH_FLAG_SUBTREE = 8626,
    /// <summary>The search flags for the attribute are invalid. The tuple index bit is valid only on attributes of Unicode strings.</summary>
    ERROR_DS_INVALID_SEARCH_FLAG_TUPLE = 8627,
    /// <summary>The address books are nested too deeply. Failed to build the hierarchy table.</summary>
    ERROR_DS_HIERARCHY_TABLE_TOO_DEEP = 8628,
    /// <summary>The specified up-to-date-ness vector is corrupt.</summary>
    ERROR_DS_DRA_CORRUPT_UTD_VECTOR = 8629,
    /// <summary>The request to replicate secrets is denied.</summary>
    ERROR_DS_DRA_SECRETS_DENIED = 8630,
    /// <summary>Schema update failed: The MAPI identifier is reserved.</summary>
    ERROR_DS_RESERVED_MAPI_ID = 8631,
    /// <summary>Schema update failed: There are no MAPI identifiers available.</summary>
    ERROR_DS_MAPI_ID_NOT_AVAILABLE = 8632,
    /// <summary>The replication operation failed because the required attributes of the local krbtgt object are missing.</summary>
    ERROR_DS_DRA_MISSING_KRBTGT_SECRET = 8633,
    /// <summary>The domain name of the trusted domain already exists in the forest.</summary>
    ERROR_DS_DOMAIN_NAME_EXISTS_IN_FOREST = 8634,
    /// <summary>The flat name of the trusted domain already exists in the forest.</summary>
    ERROR_DS_FLAT_NAME_EXISTS_IN_FOREST = 8635,
    /// <summary>The User Principal Name (UPN) is invalid.</summary>
    ERROR_INVALID_USER_PRINCIPAL_NAME = 8636,
    /// <summary>OID mapped groups cannot have members.</summary>
    ERROR_DS_OID_MAPPED_GROUP_CANT_HAVE_MEMBERS = 8637,
    /// <summary>The specified OID cannot be found.</summary>
    ERROR_DS_OID_NOT_FOUND = 8638,
    /// <summary>The replication operation failed because the target object referred by a link value is recycled.</summary>
    ERROR_DS_DRA_RECYCLED_TARGET = 8639,
    /// <summary>DNS server unable to interpret format.</summary>
    DNS_ERROR_RCODE_FORMAT_ERROR = 9001,
    /// <summary>DNS server failure.</summary>
    DNS_ERROR_RCODE_SERVER_FAILURE = 9002,
    /// <summary>DNS name does not exist.</summary>
    DNS_ERROR_RCODE_NAME_ERROR = 9003,
    /// <summary>DNS request not supported by name server.</summary>
    DNS_ERROR_RCODE_NOT_IMPLEMENTED = 9004,
    /// <summary>DNS operation refused.</summary>
    DNS_ERROR_RCODE_REFUSED = 9005,
    /// <summary>DNS name that ought not exist, does exist.</summary>
    DNS_ERROR_RCODE_YXDOMAIN = 9006,
    /// <summary>DNS RR set that ought not exist, does exist.</summary>
    DNS_ERROR_RCODE_YXRRSET = 9007,
    /// <summary>DNS RR set that ought to exist, does not exist.</summary>
    DNS_ERROR_RCODE_NXRRSET = 9008,
    /// <summary>DNS server not authoritative for zone.</summary>
    DNS_ERROR_RCODE_NOTAUTH = 9009,
    /// <summary>DNS name in update or prereq is not in zone.</summary>
    DNS_ERROR_RCODE_NOTZONE = 9010,
    /// <summary>DNS signature failed to verify.</summary>
    DNS_ERROR_RCODE_BADSIG = 9016,
    /// <summary>DNS bad key.</summary>
    DNS_ERROR_RCODE_BADKEY = 9017,
    /// <summary>DNS signature validity expired.</summary>
    DNS_ERROR_RCODE_BADTIME = 9018,
    /// <summary>No records found for given DNS query.</summary>
    DNS_INFO_NO_RECORDS = 9501,
    /// <summary>Bad DNS packet.</summary>
    DNS_ERROR_BAD_PACKET = 9502,
    /// <summary>No DNS packet.</summary>
    DNS_ERROR_NO_PACKET = 9503,
    /// <summary>DNS error, check rcode.</summary>
    DNS_ERROR_RCODE = 9504,
    /// <summary>Unsecured DNS packet.</summary>
    DNS_ERROR_UNSECURE_PACKET = 9505,
    /// <summary>Invalid DNS type.</summary>
    DNS_ERROR_INVALID_TYPE = 9551,
    /// <summary>Invalid IP address.</summary>
    DNS_ERROR_INVALID_IP_ADDRESS = 9552,
    /// <summary>Invalid property.</summary>
    DNS_ERROR_INVALID_PROPERTY = 9553,
    /// <summary>Try DNS operation again later.</summary>
    DNS_ERROR_TRY_AGAIN_LATER = 9554,
    /// <summary>Record for given name and type is not unique.</summary>
    DNS_ERROR_NOT_UNIQUE = 9555,
    /// <summary>DNS name does not comply with RFC specifications.</summary>
    DNS_ERROR_NON_RFC_NAME = 9556,
    /// <summary>DNS name is a fully-qualified DNS name.</summary>
    DNS_STATUS_FQDN = 9557,
    /// <summary>DNS name is dotted (multi-label).</summary>
    DNS_STATUS_DOTTED_NAME = 9558,
    /// <summary>DNS name is a single-part name.</summary>
    DNS_STATUS_SINGLE_PART_NAME = 9559,
    /// <summary>DNS name contains an invalid character.</summary>
    DNS_ERROR_INVALID_NAME_CHAR = 9560,
    /// <summary>DNS name is entirely numeric.</summary>
    DNS_ERROR_NUMERIC_NAME = 9561,
    /// <summary>The operation requested is not permitted on a DNS root server.</summary>
    DNS_ERROR_NOT_ALLOWED_ON_ROOT_SERVER = 9562,
    /// <summary>The record could not be created because this part of the DNS namespace has been delegated to another server.</summary>
    DNS_ERROR_NOT_ALLOWED_UNDER_DELEGATION = 9563,
    /// <summary>The DNS server could not find a set of root hints.</summary>
    DNS_ERROR_CANNOT_FIND_ROOT_HINTS = 9564,
    /// <summary>The DNS server found root hints but they were not consistent across all adapters.</summary>
    DNS_ERROR_INCONSISTENT_ROOT_HINTS = 9565,
    /// <summary>The specified value is too small for this parameter.</summary>
    DNS_ERROR_DWORD_VALUE_TOO_SMALL = 9566,
    /// <summary>The specified value is too large for this parameter.</summary>
    DNS_ERROR_DWORD_VALUE_TOO_LARGE = 9567,
    /// <summary>This operation is not allowed while the DNS server is loading zones in the background. Please try again later.</summary>
    DNS_ERROR_BACKGROUND_LOADING = 9568,
    /// <summary>The operation requested is not permitted on against a DNS server running on a read-only DC.</summary>
    DNS_ERROR_NOT_ALLOWED_ON_RODC = 9569,
    /// <summary>No data is allowed to exist underneath a DNAME record.</summary>
    DNS_ERROR_NOT_ALLOWED_UNDER_DNAME = 9570,
    /// <summary>This operation requires credentials delegation.</summary>
    DNS_ERROR_DELEGATION_REQUIRED = 9571,
    /// <summary>Name resolution policy table has been corrupted. DNS resolution will fail until it is fixed. Contact your network administrator.</summary>
    DNS_ERROR_INVALID_POLICY_TABLE = 9572,
    /// <summary>DNS zone does not exist.</summary>
    DNS_ERROR_ZONE_DOES_NOT_EXIST = 9601,
    /// <summary>DNS zone information not available.</summary>
    DNS_ERROR_NO_ZONE_INFO = 9602,
    /// <summary>Invalid operation for DNS zone.</summary>
    DNS_ERROR_INVALID_ZONE_OPERATION = 9603,
    /// <summary>Invalid DNS zone configuration.</summary>
    DNS_ERROR_ZONE_CONFIGURATION_ERROR = 9604,
    /// <summary>DNS zone has no start of authority (SOA) record.</summary>
    DNS_ERROR_ZONE_HAS_NO_SOA_RECORD = 9605,
    /// <summary>DNS zone has no Name Server (NS) record.</summary>
    DNS_ERROR_ZONE_HAS_NO_NS_RECORDS = 9606,
    /// <summary>DNS zone is locked.</summary>
    DNS_ERROR_ZONE_LOCKED = 9607,
    /// <summary>DNS zone creation failed.</summary>
    DNS_ERROR_ZONE_CREATION_FAILED = 9608,
    /// <summary>DNS zone already exists.</summary>
    DNS_ERROR_ZONE_ALREADY_EXISTS = 9609,
    /// <summary>DNS automatic zone already exists.</summary>
    DNS_ERROR_AUTOZONE_ALREADY_EXISTS = 9610,
    /// <summary>Invalid DNS zone type.</summary>
    DNS_ERROR_INVALID_ZONE_TYPE = 9611,
    /// <summary>Secondary DNS zone requires master IP address.</summary>
    DNS_ERROR_SECONDARY_REQUIRES_MASTER_IP = 9612,
    /// <summary>DNS zone not secondary.</summary>
    DNS_ERROR_ZONE_NOT_SECONDARY = 9613,
    /// <summary>Need secondary IP address.</summary>
    DNS_ERROR_NEED_SECONDARY_ADDRESSES = 9614,
    /// <summary>WINS initialization failed.</summary>
    DNS_ERROR_WINS_INIT_FAILED = 9615,
    /// <summary>Need WINS servers.</summary>
    DNS_ERROR_NEED_WINS_SERVERS = 9616,
    /// <summary>NBTSTAT initialization call failed.</summary>
    DNS_ERROR_NBSTAT_INIT_FAILED = 9617,
    /// <summary>Invalid delete of start of authority (SOA)</summary>
    DNS_ERROR_SOA_DELETE_INVALID = 9618,
    /// <summary>A conditional forwarding zone already exists for that name.</summary>
    DNS_ERROR_FORWARDER_ALREADY_EXISTS = 9619,
    /// <summary>This zone must be configured with one or more master DNS server IP addresses.</summary>
    DNS_ERROR_ZONE_REQUIRES_MASTER_IP = 9620,
    /// <summary>The operation cannot be performed because this zone is shutdown.</summary>
    DNS_ERROR_ZONE_IS_SHUTDOWN = 9621,
    /// <summary>Primary DNS zone requires datafile.</summary>
    DNS_ERROR_PRIMARY_REQUIRES_DATAFILE = 9651,
    /// <summary>Invalid datafile name for DNS zone.</summary>
    DNS_ERROR_INVALID_DATAFILE_NAME = 9652,
    /// <summary>Failed to open datafile for DNS zone.</summary>
    DNS_ERROR_DATAFILE_OPEN_FAILURE = 9653,
    /// <summary>Failed to write datafile for DNS zone.</summary>
    DNS_ERROR_FILE_WRITEBACK_FAILED = 9654,
    /// <summary>Failure while reading datafile for DNS zone.</summary>
    DNS_ERROR_DATAFILE_PARSING = 9655,
    /// <summary>DNS record does not exist.</summary>
    DNS_ERROR_RECORD_DOES_NOT_EXIST = 9701,
    /// <summary>DNS record format error.</summary>
    DNS_ERROR_RECORD_FORMAT = 9702,
    /// <summary>Node creation failure in DNS.</summary>
    DNS_ERROR_NODE_CREATION_FAILED = 9703,
    /// <summary>Unknown DNS record type.</summary>
    DNS_ERROR_UNKNOWN_RECORD_TYPE = 9704,
    /// <summary>DNS record timed out.</summary>
    DNS_ERROR_RECORD_TIMED_OUT = 9705,
    /// <summary>Name not in DNS zone.</summary>
    DNS_ERROR_NAME_NOT_IN_ZONE = 9706,
    /// <summary>CNAME loop detected.</summary>
    DNS_ERROR_CNAME_LOOP = 9707,
    /// <summary>Node is a CNAME DNS record.</summary>
    DNS_ERROR_NODE_IS_CNAME = 9708,
    /// <summary>A CNAME record already exists for given name.</summary>
    DNS_ERROR_CNAME_COLLISION = 9709,
    /// <summary>Record only at DNS zone root.</summary>
    DNS_ERROR_RECORD_ONLY_AT_ZONE_ROOT = 9710,
    /// <summary>DNS record already exists.</summary>
    DNS_ERROR_RECORD_ALREADY_EXISTS = 9711,
    /// <summary>Secondary DNS zone data error.</summary>
    DNS_ERROR_SECONDARY_DATA = 9712,
    /// <summary>Could not create DNS cache data.</summary>
    DNS_ERROR_NO_CREATE_CACHE_DATA = 9713,
    /// <summary>DNS name does not exist.</summary>
    DNS_ERROR_NAME_DOES_NOT_EXIST = 9714,
    /// <summary>Could not create pointer (PTR) record.</summary>
    DNS_WARNING_PTR_CREATE_FAILED = 9715,
    /// <summary>DNS domain was undeleted.</summary>
    DNS_WARNING_DOMAIN_UNDELETED = 9716,
    /// <summary>The directory service is unavailable.</summary>
    DNS_ERROR_DS_UNAVAILABLE = 9717,
    /// <summary>DNS zone already exists in the directory service.</summary>
    DNS_ERROR_DS_ZONE_ALREADY_EXISTS = 9718,
    /// <summary>DNS server not creating or reading the boot file for the directory service integrated DNS zone.</summary>
    DNS_ERROR_NO_BOOTFILE_IF_DS_ZONE = 9719,
    /// <summary>Node is a DNAME DNS record.</summary>
    DNS_ERROR_NODE_IS_DNAME = 9720,
    /// <summary>A DNAME record already exists for given name.</summary>
    DNS_ERROR_DNAME_COLLISION = 9721,
    /// <summary>An alias loop has been detected with either CNAME or DNAME records.</summary>
    DNS_ERROR_ALIAS_LOOP = 9722,
    /// <summary>DNS AXFR (zone transfer) complete.</summary>
    DNS_INFO_AXFR_COMPLETE = 9751,
    /// <summary>DNS zone transfer failed.</summary>
    DNS_ERROR_AXFR = 9752,
    /// <summary>Added local WINS server.</summary>
    DNS_INFO_ADDED_LOCAL_WINS = 9753,
    /// <summary>Secure update call needs to continue update request.</summary>
    DNS_STATUS_CONTINUE_NEEDED = 9801,
    /// <summary>TCP/IP network protocol not installed.</summary>
    DNS_ERROR_NO_TCPIP = 9851,
    /// <summary>No DNS servers configured for local system.</summary>
    DNS_ERROR_NO_DNS_SERVERS = 9852,
    /// <summary>The specified directory partition does not exist.</summary>
    DNS_ERROR_DP_DOES_NOT_EXIST = 9901,
    /// <summary>The specified directory partition already exists.</summary>
    DNS_ERROR_DP_ALREADY_EXISTS = 9902,
    /// <summary>This DNS server is not enlisted in the specified directory partition.</summary>
    DNS_ERROR_DP_NOT_ENLISTED = 9903,
    /// <summary>This DNS server is already enlisted in the specified directory partition.</summary>
    DNS_ERROR_DP_ALREADY_ENLISTED = 9904,
    /// <summary>The directory partition is not available at this time. Please wait a few minutes and try again.</summary>
    DNS_ERROR_DP_NOT_AVAILABLE = 9905,
    /// <summary>The application directory partition operation failed. The domain controller holding the domain naming master role is down or unable to service the request or is not running Windows Server 2003.</summary>
    DNS_ERROR_DP_FSMO_ERROR = 9906,
    /// <summary>A blocking operation was interrupted by a call to WSACancelBlockingCall.</summary>
    WSAEINTR = 10004,
    /// <summary>The file handle supplied is not valid.</summary>
    WSAEBADF = 10009,
    /// <summary>An attempt was made to access a socket in a way forbidden by its access permissions.</summary>
    WSAEACCES = 10013,
    /// <summary>The system detected an invalid pointer address in attempting to use a pointer argument in a call.</summary>
    WSAEFAULT = 10014,
    /// <summary>An invalid argument was supplied.</summary>
    WSAEINVAL = 10022,
    /// <summary>Too many open sockets.</summary>
    WSAEMFILE = 10024,
    /// <summary>A non-blocking socket operation could not be completed immediately.</summary>
    WSAEWOULDBLOCK = 10035,
    /// <summary>A blocking operation is currently executing.</summary>
    WSAEINPROGRESS = 10036,
    /// <summary>An operation was attempted on a non-blocking socket that already had an operation in progress.</summary>
    WSAEALREADY = 10037,
    /// <summary>An operation was attempted on something that is not a socket.</summary>
    WSAENOTSOCK = 10038,
    /// <summary>A required address was omitted from an operation on a socket.</summary>
    WSAEDESTADDRREQ = 10039,
    /// <summary>A message sent on a datagram socket was larger than the internal message buffer or some other network limit, or the buffer used to receive a datagram into was smaller than the datagram itself.</summary>
    WSAEMSGSIZE = 10040,
    /// <summary>A protocol was specified in the socket function call that does not support the semantics of the socket type requested.</summary>
    WSAEPROTOTYPE = 10041,
    /// <summary>An unknown, invalid, or unsupported option or level was specified in a getsockopt or setsockopt call.</summary>
    WSAENOPROTOOPT = 10042,
    /// <summary>The requested protocol has not been configured into the system, or no implementation for it exists.</summary>
    WSAEPROTONOSUPPORT = 10043,
    /// <summary>The support for the specified socket type does not exist in this address family.</summary>
    WSAESOCKTNOSUPPORT = 10044,
    /// <summary>The attempted operation is not supported for the type of object referenced.</summary>
    WSAEOPNOTSUPP = 10045,
    /// <summary>The protocol family has not been configured into the system or no implementation for it exists.</summary>
    WSAEPFNOSUPPORT = 10046,
    /// <summary>An address incompatible with the requested protocol was used.</summary>
    WSAEAFNOSUPPORT = 10047,
    /// <summary>Only one usage of each socket address (protocol/network address/port) is normally permitted.</summary>
    WSAEADDRINUSE = 10048,
    /// <summary>The requested address is not valid in its context.</summary>
    WSAEADDRNOTAVAIL = 10049,
    /// <summary>A socket operation encountered a dead network.</summary>
    WSAENETDOWN = 10050,
    /// <summary>A socket operation was attempted to an unreachable network.</summary>
    WSAENETUNREACH = 10051,
    /// <summary>The connection has been broken due to keep-alive activity detecting a failure while the operation was in progress.</summary>
    WSAENETRESET = 10052,
    /// <summary>An established connection was aborted by the software in your host machine.</summary>
    WSAECONNABORTED = 10053,
    /// <summary>An existing connection was forcibly closed by the remote host.</summary>
    WSAECONNRESET = 10054,
    /// <summary>An operation on a socket could not be performed because the system lacked sufficient buffer space or because a queue was full.</summary>
    WSAENOBUFS = 10055,
    /// <summary>A connect request was made on an already connected socket.</summary>
    WSAEISCONN = 10056,
    /// <summary>A request to send or receive data was disallowed because the socket is not connected and (when sending on a datagram socket using a sendto call) no address was supplied.</summary>
    WSAENOTCONN = 10057,
    /// <summary>A request to send or receive data was disallowed because the socket had already been shut down in that direction with a previous shutdown call.</summary>
    WSAESHUTDOWN = 10058,
    /// <summary>Too many references to some kernel object.</summary>
    WSAETOOMANYREFS = 10059,
    /// <summary>A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond.</summary>
    WSAETIMEDOUT = 10060,
    /// <summary>No connection could be made because the target machine actively refused it.</summary>
    WSAECONNREFUSED = 10061,
    /// <summary>Cannot translate name.</summary>
    WSAELOOP = 10062,
    /// <summary>Name component or name was too long.</summary>
    WSAENAMETOOLONG = 10063,
    /// <summary>A socket operation failed because the destination host was down.</summary>
    WSAEHOSTDOWN = 10064,
    /// <summary>A socket operation was attempted to an unreachable host.</summary>
    WSAEHOSTUNREACH = 10065,
    /// <summary>Cannot remove a directory that is not empty.</summary>
    WSAENOTEMPTY = 10066,
    /// <summary>A Windows Sockets implementation may have a limit on the number of applications that may use it simultaneously.</summary>
    WSAEPROCLIM = 10067,
    /// <summary>Ran out of quota.</summary>
    WSAEUSERS = 10068,
    /// <summary>Ran out of disk quota.</summary>
    WSAEDQUOT = 10069,
    /// <summary>File handle reference is no longer available.</summary>
    WSAESTALE = 10070,
    /// <summary>Item is not available locally.</summary>
    WSAEREMOTE = 10071,
    /// <summary>WSAStartup cannot function at this time because the underlying system it uses to provide network services is currently unavailable.</summary>
    WSASYSNOTREADY = 10091,
    /// <summary>The Windows Sockets version requested is not supported.</summary>
    WSAVERNOTSUPPORTED = 10092,
    /// <summary>Either the application has not called WSAStartup, or WSAStartup failed.</summary>
    WSANOTINITIALISED = 10093,
    /// <summary>Returned by WSARecv or WSARecvFrom to indicate the remote party has initiated a graceful shutdown sequence.</summary>
    WSAEDISCON = 10101,
    /// <summary>No more results can be returned by WSALookupServiceNext.</summary>
    WSAENOMORE = 10102,
    /// <summary>A call to WSALookupServiceEnd was made while this call was still processing. The call has been canceled.</summary>
    WSAECANCELLED = 10103,
    /// <summary>The procedure call table is invalid.</summary>
    WSAEINVALIDPROCTABLE = 10104,
    /// <summary>The requested service provider is invalid.</summary>
    WSAEINVALIDPROVIDER = 10105,
    /// <summary>The requested service provider could not be loaded or initialized.</summary>
    WSAEPROVIDERFAILEDINIT = 10106,
    /// <summary>A system call has failed.</summary>
    WSASYSCALLFAILURE = 10107,
    /// <summary>No such service is known. The service cannot be found in the specified name space.</summary>
    WSASERVICE_NOT_FOUND = 10108,
    /// <summary>The specified class was not found.</summary>
    WSATYPE_NOT_FOUND = 10109,
    /// <summary>No more results can be returned by WSALookupServiceNext.</summary>
    WSA_E_NO_MORE = 10110,
    /// <summary>A call to WSALookupServiceEnd was made while this call was still processing. The call has been canceled.</summary>
    WSA_E_CANCELLED = 10111,
    /// <summary>A database query failed because it was actively refused.</summary>
    WSAEREFUSED = 10112,
    /// <summary>No such host is known.</summary>
    WSAHOST_NOT_FOUND = 11001,
    /// <summary>This is usually a temporary error during hostname resolution and means that the local server did not receive a response from an authoritative server.</summary>
    WSATRY_AGAIN = 11002,
    /// <summary>A non-recoverable error occurred during a database lookup.</summary>
    WSANO_RECOVERY = 11003,
    /// <summary>The requested name is valid, but no data of the requested type was found.</summary>
    WSANO_DATA = 11004,
    /// <summary>At least one reserve has arrived.</summary>
    WSA_QOS_RECEIVERS = 11005,
    /// <summary>At least one path has arrived.</summary>
    WSA_QOS_SENDERS = 11006,
    /// <summary>There are no senders.</summary>
    WSA_QOS_NO_SENDERS = 11007,
    /// <summary>There are no receivers.</summary>
    WSA_QOS_NO_RECEIVERS = 11008,
    /// <summary>Reserve has been confirmed.</summary>
    WSA_QOS_REQUEST_CONFIRMED = 11009,
    /// <summary>Error due to lack of resources.</summary>
    WSA_QOS_ADMISSION_FAILURE = 11010,
    /// <summary>Rejected for administrative reasons - bad credentials.</summary>
    WSA_QOS_POLICY_FAILURE = 11011,
    /// <summary>Unknown or conflicting style.</summary>
    WSA_QOS_BAD_STYLE = 11012,
    /// <summary>Problem with some part of the filterspec or providerspecific buffer in general.</summary>
    WSA_QOS_BAD_OBJECT = 11013,
    /// <summary>Problem with some part of the flowspec.</summary>
    WSA_QOS_TRAFFIC_CTRL_ERROR = 11014,
    /// <summary>General QOS error.</summary>
    WSA_QOS_GENERIC_ERROR = 11015,
    /// <summary>An invalid or unrecognized service type was found in the flowspec.</summary>
    WSA_QOS_ESERVICETYPE = 11016,
    /// <summary>An invalid or inconsistent flowspec was found in the QOS structure.</summary>
    WSA_QOS_EFLOWSPEC = 11017,
    /// <summary>Invalid QOS provider-specific buffer.</summary>
    WSA_QOS_EPROVSPECBUF = 11018,
    /// <summary>An invalid QOS filter style was used.</summary>
    WSA_QOS_EFILTERSTYLE = 11019,
    /// <summary>An invalid QOS filter type was used.</summary>
    WSA_QOS_EFILTERTYPE = 11020,
    /// <summary>An incorrect number of QOS FILTERSPECs were specified in the FLOWDESCRIPTOR.</summary>
    WSA_QOS_EFILTERCOUNT = 11021,
    /// <summary>An object with an invalid ObjectLength field was specified in the QOS provider-specific buffer.</summary>
    WSA_QOS_EOBJLENGTH = 11022,
    /// <summary>An incorrect number of flow descriptors was specified in the QOS structure.</summary>
    WSA_QOS_EFLOWCOUNT = 11023,
    /// <summary>An unrecognized object was found in the QOS provider-specific buffer.</summary>
    WSA_QOS_EUNKOWNPSOBJ = 11024,
    /// <summary>An invalid policy object was found in the QOS provider-specific buffer.</summary>
    WSA_QOS_EPOLICYOBJ = 11025,
    /// <summary>An invalid QOS flow descriptor was found in the flow descriptor list.</summary>
    WSA_QOS_EFLOWDESC = 11026,
    /// <summary>An invalid or inconsistent flowspec was found in the QOS provider specific buffer.</summary>
    WSA_QOS_EPSFLOWSPEC = 11027,
    /// <summary>An invalid FILTERSPEC was found in the QOS provider-specific buffer.</summary>
    WSA_QOS_EPSFILTERSPEC = 11028,
    /// <summary>An invalid shape discard mode object was found in the QOS provider specific buffer.</summary>
    WSA_QOS_ESDMODEOBJ = 11029,
    /// <summary>An invalid shaping rate object was found in the QOS provider-specific buffer.</summary>
    WSA_QOS_ESHAPERATEOBJ = 11030,
    /// <summary>A reserved policy element was found in the QOS provider-specific buffer.</summary>
    WSA_QOS_RESERVED_PETYPE = 11031,
    /// <summary>No such host is known securely.</summary>
    WSA_SECURE_HOST_NOT_FOUND = 11032,
    /// <summary>Name based IPSEC policy could not be added.</summary>
    WSA_IPSEC_NAME_POLICY_ERROR = 11033,
    /// <summary>The specified quick mode policy already exists.</summary>
    ERROR_IPSEC_QM_POLICY_EXISTS = 13000,
    /// <summary>The specified quick mode policy was not found.</summary>
    ERROR_IPSEC_QM_POLICY_NOT_FOUND = 13001,
    /// <summary>The specified quick mode policy is being used.</summary>
    ERROR_IPSEC_QM_POLICY_IN_USE = 13002,
    /// <summary>The specified main mode policy already exists.</summary>
    ERROR_IPSEC_MM_POLICY_EXISTS = 13003,
    /// <summary>The specified main mode policy was not found</summary>
    ERROR_IPSEC_MM_POLICY_NOT_FOUND = 13004,
    /// <summary>The specified main mode policy is being used.</summary>
    ERROR_IPSEC_MM_POLICY_IN_USE = 13005,
    /// <summary>The specified main mode filter already exists.</summary>
    ERROR_IPSEC_MM_FILTER_EXISTS = 13006,
    /// <summary>The specified main mode filter was not found.</summary>
    ERROR_IPSEC_MM_FILTER_NOT_FOUND = 13007,
    /// <summary>The specified transport mode filter already exists.</summary>
    ERROR_IPSEC_TRANSPORT_FILTER_EXISTS = 13008,
    /// <summary>The specified transport mode filter does not exist.</summary>
    ERROR_IPSEC_TRANSPORT_FILTER_NOT_FOUND = 13009,
    /// <summary>The specified main mode authentication list exists.</summary>
    ERROR_IPSEC_MM_AUTH_EXISTS = 13010,
    /// <summary>The specified main mode authentication list was not found.</summary>
    ERROR_IPSEC_MM_AUTH_NOT_FOUND = 13011,
    /// <summary>The specified main mode authentication list is being used.</summary>
    ERROR_IPSEC_MM_AUTH_IN_USE = 13012,
    /// <summary>The specified default main mode policy was not found.</summary>
    ERROR_IPSEC_DEFAULT_MM_POLICY_NOT_FOUND = 13013,
    /// <summary>The specified default main mode authentication list was not found.</summary>
    ERROR_IPSEC_DEFAULT_MM_AUTH_NOT_FOUND = 13014,
    /// <summary>The specified default quick mode policy was not found.</summary>
    ERROR_IPSEC_DEFAULT_QM_POLICY_NOT_FOUND = 13015,
    /// <summary>The specified tunnel mode filter exists.</summary>
    ERROR_IPSEC_TUNNEL_FILTER_EXISTS = 13016,
    /// <summary>The specified tunnel mode filter was not found.</summary>
    ERROR_IPSEC_TUNNEL_FILTER_NOT_FOUND = 13017,
    /// <summary>The Main Mode filter is pending deletion.</summary>
    ERROR_IPSEC_MM_FILTER_PENDING_DELETION = 13018,
    /// <summary>The transport filter is pending deletion.</summary>
    ERROR_IPSEC_TRANSPORT_FILTER_PENDING_DELETION = 13019,
    /// <summary>The tunnel filter is pending deletion.</summary>
    ERROR_IPSEC_TUNNEL_FILTER_PENDING_DELETION = 13020,
    /// <summary>The Main Mode policy is pending deletion.</summary>
    ERROR_IPSEC_MM_POLICY_PENDING_DELETION = 13021,
    /// <summary>The Main Mode authentication bundle is pending deletion.</summary>
    ERROR_IPSEC_MM_AUTH_PENDING_DELETION = 13022,
    /// <summary>The Quick Mode policy is pending deletion.</summary>
    ERROR_IPSEC_QM_POLICY_PENDING_DELETION = 13023,
    /// <summary>The Main Mode policy was successfully added, but some of the requested offers are not supported.</summary>
    WARNING_IPSEC_MM_POLICY_PRUNED = 13024,
    /// <summary>The Quick Mode policy was successfully added, but some of the requested offers are not supported.</summary>
    WARNING_IPSEC_QM_POLICY_PRUNED = 13025,
    /// <summary> ERROR_IPSEC_IKE_NEG_STATUS_BEGIN</summary>
    ERROR_IPSEC_IKE_NEG_STATUS_BEGIN = 13800,
    /// <summary>IKE authentication credentials are unacceptable</summary>
    ERROR_IPSEC_IKE_AUTH_FAIL = 13801,
    /// <summary>IKE security attributes are unacceptable</summary>
    ERROR_IPSEC_IKE_ATTRIB_FAIL = 13802,
    /// <summary>IKE Negotiation in progress</summary>
    ERROR_IPSEC_IKE_NEGOTIATION_PENDING = 13803,
    /// <summary>General processing error</summary>
    ERROR_IPSEC_IKE_GENERAL_PROCESSING_ERROR = 13804,
    /// <summary>Negotiation timed out</summary>
    ERROR_IPSEC_IKE_TIMED_OUT = 13805,
    /// <summary>IKE failed to find valid machine certificate. Contact your Network Security Administrator about installing a valid certificate in the appropriate Certificate Store.</summary>
    ERROR_IPSEC_IKE_NO_CERT = 13806,
    /// <summary>IKE SA deleted by peer before establishment completed</summary>
    ERROR_IPSEC_IKE_SA_DELETED = 13807,
    /// <summary>IKE SA deleted before establishment completed</summary>
    ERROR_IPSEC_IKE_SA_REAPED = 13808,
    /// <summary>Negotiation request sat in Queue too long</summary>
    ERROR_IPSEC_IKE_MM_ACQUIRE_DROP = 13809,
    /// <summary>Negotiation request sat in Queue too long</summary>
    ERROR_IPSEC_IKE_QM_ACQUIRE_DROP = 13810,
    /// <summary>Negotiation request sat in Queue too long</summary>
    ERROR_IPSEC_IKE_QUEUE_DROP_MM = 13811,
    /// <summary>Negotiation request sat in Queue too long</summary>
    ERROR_IPSEC_IKE_QUEUE_DROP_NO_MM = 13812,
    /// <summary>No response from peer</summary>
    ERROR_IPSEC_IKE_DROP_NO_RESPONSE = 13813,
    /// <summary>Negotiation took too long</summary>
    ERROR_IPSEC_IKE_MM_DELAY_DROP = 13814,
    /// <summary>Negotiation took too long</summary>
    ERROR_IPSEC_IKE_QM_DELAY_DROP = 13815,
    /// <summary>Unknown error occurred</summary>
    ERROR_IPSEC_IKE_ERROR = 13816,
    /// <summary>Certificate Revocation Check failed</summary>
    ERROR_IPSEC_IKE_CRL_FAILED = 13817,
    /// <summary>Invalid certificate key usage</summary>
    ERROR_IPSEC_IKE_INVALID_KEY_USAGE = 13818,
    /// <summary>Invalid certificate type</summary>
    ERROR_IPSEC_IKE_INVALID_CERT_TYPE = 13819,
    /// <summary>IKE negotiation failed because the machine certificate used does not have a private key. IPsec certificates require a private key. Contact your Network Security administrator about replacing with a certificate that has a private key.</summary>
    ERROR_IPSEC_IKE_NO_PRIVATE_KEY = 13820,
    /// <summary>Simultaneous rekeys were detected.</summary>
    ERROR_IPSEC_IKE_SIMULTANEOUS_REKEY = 13821,
    /// <summary>Failure in Diffie-Hellman computation</summary>
    ERROR_IPSEC_IKE_DH_FAIL = 13822,
    /// <summary>Don&#39;t know how to process critical payload</summary>
    ERROR_IPSEC_IKE_CRITICAL_PAYLOAD_NOT_RECOGNIZED = 13823,
    /// <summary>Invalid header</summary>
    ERROR_IPSEC_IKE_INVALID_HEADER = 13824,
    /// <summary>No policy configured</summary>
    ERROR_IPSEC_IKE_NO_POLICY = 13825,
    /// <summary>Failed to verify signature</summary>
    ERROR_IPSEC_IKE_INVALID_SIGNATURE = 13826,
    /// <summary>Failed to authenticate using Kerberos</summary>
    ERROR_IPSEC_IKE_KERBEROS_ERROR = 13827,
    /// <summary>Peer&#39;s certificate did not have a public key</summary>
    ERROR_IPSEC_IKE_NO_PUBLIC_KEY = 13828,
    /// <summary>Error processing error payload</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR = 13829,
    /// <summary>Error processing SA payload</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR_SA = 13830,
    /// <summary>Error processing Proposal payload</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR_PROP = 13831,
    /// <summary>Error processing Transform payload</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR_TRANS = 13832,
    /// <summary>Error processing KE payload</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR_KE = 13833,
    /// <summary>Error processing ID payload</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR_ID = 13834,
    /// <summary>Error processing Cert payload</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR_CERT = 13835,
    /// <summary>Error processing Certificate Request payload</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR_CERT_REQ = 13836,
    /// <summary>Error processing Hash payload</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR_HASH = 13837,
    /// <summary>Error processing Signature payload</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR_SIG = 13838,
    /// <summary>Error processing Nonce payload</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR_NONCE = 13839,
    /// <summary>Error processing Notify payload</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR_NOTIFY = 13840,
    /// <summary>Error processing Delete Payload</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR_DELETE = 13841,
    /// <summary>Error processing VendorId payload</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR_VENDOR = 13842,
    /// <summary>Invalid payload received</summary>
    ERROR_IPSEC_IKE_INVALID_PAYLOAD = 13843,
    /// <summary>Soft SA loaded</summary>
    ERROR_IPSEC_IKE_LOAD_SOFT_SA = 13844,
    /// <summary>Soft SA torn down</summary>
    ERROR_IPSEC_IKE_SOFT_SA_TORN_DOWN = 13845,
    /// <summary>Invalid cookie received.</summary>
    ERROR_IPSEC_IKE_INVALID_COOKIE = 13846,
    /// <summary>Peer failed to send valid machine certificate</summary>
    ERROR_IPSEC_IKE_NO_PEER_CERT = 13847,
    /// <summary>Certification Revocation check of peer&#39;s certificate failed</summary>
    ERROR_IPSEC_IKE_PEER_CRL_FAILED = 13848,
    /// <summary>New policy invalidated SAs formed with old policy</summary>
    ERROR_IPSEC_IKE_POLICY_CHANGE = 13849,
    /// <summary>There is no available Main Mode IKE policy.</summary>
    ERROR_IPSEC_IKE_NO_MM_POLICY = 13850,
    /// <summary>Failed to enabled TCB privilege.</summary>
    ERROR_IPSEC_IKE_NOTCBPRIV = 13851,
    /// <summary>Failed to load SECURITY.DLL.</summary>
    ERROR_IPSEC_IKE_SECLOADFAIL = 13852,
    /// <summary>Failed to obtain security function table dispatch address from SSPI.</summary>
    ERROR_IPSEC_IKE_FAILSSPINIT = 13853,
    /// <summary>Failed to query Kerberos package to obtain max token size.</summary>
    ERROR_IPSEC_IKE_FAILQUERYSSP = 13854,
    /// <summary>Failed to obtain Kerberos server credentials for ISAKMP/ERROR_IPSEC_IKE service. Kerberos authentication will not function. The most likely reason for this is lack of domain membership. This is normal if your computer is a member of a workgroup.</summary>
    ERROR_IPSEC_IKE_SRVACQFAIL = 13855,
    /// <summary>Failed to determine SSPI principal name for ISAKMP/ERROR_IPSEC_IKE service (QueryCredentialsAttributes).</summary>
    ERROR_IPSEC_IKE_SRVQUERYCRED = 13856,
    /// <summary>Failed to obtain new SPI for the inbound SA from Ipsec driver. The most common cause for this is that the driver does not have the correct filter. Check your policy to verify the filters.</summary>
    ERROR_IPSEC_IKE_GETSPIFAIL = 13857,
    /// <summary>Given filter is invalid</summary>
    ERROR_IPSEC_IKE_INVALID_FILTER = 13858,
    /// <summary>Memory allocation failed.</summary>
    ERROR_IPSEC_IKE_OUT_OF_MEMORY = 13859,
    /// <summary>Failed to add Security Association to IPSec Driver. The most common cause for this is if the IKE negotiation took too long to complete. If the problem persists, reduce the load on the faulting machine.</summary>
    ERROR_IPSEC_IKE_ADD_UPDATE_KEY_FAILED = 13860,
    /// <summary>Invalid policy</summary>
    ERROR_IPSEC_IKE_INVALID_POLICY = 13861,
    /// <summary>Invalid DOI</summary>
    ERROR_IPSEC_IKE_UNKNOWN_DOI = 13862,
    /// <summary>Invalid situation</summary>
    ERROR_IPSEC_IKE_INVALID_SITUATION = 13863,
    /// <summary>Diffie-Hellman failure</summary>
    ERROR_IPSEC_IKE_DH_FAILURE = 13864,
    /// <summary>Invalid Diffie-Hellman group</summary>
    ERROR_IPSEC_IKE_INVALID_GROUP = 13865,
    /// <summary>Error encrypting payload</summary>
    ERROR_IPSEC_IKE_ENCRYPT = 13866,
    /// <summary>Error decrypting payload</summary>
    ERROR_IPSEC_IKE_DECRYPT = 13867,
    /// <summary>Policy match error</summary>
    ERROR_IPSEC_IKE_POLICY_MATCH = 13868,
    /// <summary>Unsupported ID</summary>
    ERROR_IPSEC_IKE_UNSUPPORTED_ID = 13869,
    /// <summary>Hash verification failed</summary>
    ERROR_IPSEC_IKE_INVALID_HASH = 13870,
    /// <summary>Invalid hash algorithm</summary>
    ERROR_IPSEC_IKE_INVALID_HASH_ALG = 13871,
    /// <summary>Invalid hash size</summary>
    ERROR_IPSEC_IKE_INVALID_HASH_SIZE = 13872,
    /// <summary>Invalid encryption algorithm</summary>
    ERROR_IPSEC_IKE_INVALID_ENCRYPT_ALG = 13873,
    /// <summary>Invalid authentication algorithm</summary>
    ERROR_IPSEC_IKE_INVALID_AUTH_ALG = 13874,
    /// <summary>Invalid certificate signature</summary>
    ERROR_IPSEC_IKE_INVALID_SIG = 13875,
    /// <summary>Load failed</summary>
    ERROR_IPSEC_IKE_LOAD_FAILED = 13876,
    /// <summary>Deleted via RPC call</summary>
    ERROR_IPSEC_IKE_RPC_DELETE = 13877,
    /// <summary>Temporary state created to perform reinit. This is not a real failure.</summary>
    ERROR_IPSEC_IKE_BENIGN_REINIT = 13878,
    /// <summary>The lifetime value received in the Responder Lifetime Notify is below the Windows 2000 configured minimum value. Please fix the policy on the peer machine.</summary>
    ERROR_IPSEC_IKE_INVALID_RESPONDER_LIFETIME_NOTIFY = 13879,
    /// <summary>The recipient cannot handle version of IKE specified in the header.</summary>
    ERROR_IPSEC_IKE_INVALID_MAJOR_VERSION = 13880,
    /// <summary>Key length in certificate is too small for configured security requirements.</summary>
    ERROR_IPSEC_IKE_INVALID_CERT_KEYLEN = 13881,
    /// <summary>Max number of established MM SAs to peer exceeded.</summary>
    ERROR_IPSEC_IKE_MM_LIMIT = 13882,
    /// <summary>IKE received a policy that disables negotiation.</summary>
    ERROR_IPSEC_IKE_NEGOTIATION_DISABLED = 13883,
    /// <summary>Reached maximum quick mode limit for the main mode. New main mode will be started.</summary>
    ERROR_IPSEC_IKE_QM_LIMIT = 13884,
    /// <summary>Main mode SA lifetime expired or peer sent a main mode delete.</summary>
    ERROR_IPSEC_IKE_MM_EXPIRED = 13885,
    /// <summary>Main mode SA assumed to be invalid because peer stopped responding.</summary>
    ERROR_IPSEC_IKE_PEER_MM_ASSUMED_INVALID = 13886,
    /// <summary>Certificate doesn&#39;t chain to a trusted root in IPsec policy.</summary>
    ERROR_IPSEC_IKE_CERT_CHAIN_POLICY_MISMATCH = 13887,
    /// <summary>Received unexpected message ID.</summary>
    ERROR_IPSEC_IKE_UNEXPECTED_MESSAGE_ID = 13888,
    /// <summary>Received invalid authentication offers.</summary>
    ERROR_IPSEC_IKE_INVALID_AUTH_PAYLOAD = 13889,
    /// <summary>Sent DoS cookie notify to initiator.</summary>
    ERROR_IPSEC_IKE_DOS_COOKIE_SENT = 13890,
    /// <summary>IKE service is shutting down.</summary>
    ERROR_IPSEC_IKE_SHUTTING_DOWN = 13891,
    /// <summary>Could not verify binding between CGA address and certificate.</summary>
    ERROR_IPSEC_IKE_CGA_AUTH_FAILED = 13892,
    /// <summary>Error processing NatOA payload.</summary>
    ERROR_IPSEC_IKE_PROCESS_ERR_NATOA = 13893,
    /// <summary>Parameters of the main mode are invalid for this quick mode.</summary>
    ERROR_IPSEC_IKE_INVALID_MM_FOR_QM = 13894,
    /// <summary>Quick mode SA was expired by IPsec driver.</summary>
    ERROR_IPSEC_IKE_QM_EXPIRED = 13895,
    /// <summary>Too many dynamically added IKEEXT filters were detected.</summary>
    ERROR_IPSEC_IKE_TOO_MANY_FILTERS = 13896,
    /// <summary> ERROR_IPSEC_IKE_NEG_STATUS_END</summary>
    ERROR_IPSEC_IKE_NEG_STATUS_END = 13897,
    /// <summary>NAP reauth succeeded and must delete the dummy NAP IkeV2 tunnel.</summary>
    ERROR_IPSEC_IKE_KILL_DUMMY_NAP_TUNNEL = 13898,
    /// <summary>Error in assigning inner IP address to intiator in tunnel mode.</summary>
    ERROR_IPSEC_IKE_INNER_IP_ASSIGNMENT_FAILURE = 13899,
    /// <summary>Require configuration payload missing.</summary>
    ERROR_IPSEC_IKE_REQUIRE_CP_PAYLOAD_MISSING = 13900,
    /// <summary>A negotiation running as the security principle who issued the connection is in progress</summary>
    ERROR_IPSEC_KEY_MODULE_IMPERSONATION_NEGOTIATION_PENDING = 13901,
    /// <summary>SA was deleted due to IKEv1/AuthIP co-existence suppress check.</summary>
    ERROR_IPSEC_IKE_COEXISTENCE_SUPPRESS = 13902,
    /// <summary>Incoming SA request was dropped due to peer IP address rate limiting.</summary>
    ERROR_IPSEC_IKE_RATELIMIT_DROP = 13903,
    /// <summary>Peer does not support MOBIKE.</summary>
    ERROR_IPSEC_IKE_PEER_DOESNT_SUPPORT_MOBIKE = 13904,
    /// <summary>SA establishment is not authorized.</summary>
    ERROR_IPSEC_IKE_AUTHORIZATION_FAILURE = 13905,
    /// <summary>SA establishment is not authorized because there is not a sufficiently strong PKINIT-based credential.</summary>
    ERROR_IPSEC_IKE_STRONG_CRED_AUTHORIZATION_FAILURE = 13906,
    /// <summary>SA establishment is not authorized.  You may need to enter updated or different credentials such as a smartcard.</summary>
    ERROR_IPSEC_IKE_AUTHORIZATION_FAILURE_WITH_OPTIONAL_RETRY = 13907,
    /// <summary>SA establishment is not authorized because there is not a sufficiently strong PKINIT-based credential. This might be related to certificate-to-account mapping failure for the SA.</summary>
    ERROR_IPSEC_IKE_STRONG_CRED_AUTHORIZATION_AND_CERTMAP_FAILURE = 13908,
    /// <summary> ERROR_IPSEC_IKE_NEG_STATUS_EXTENDED_END</summary>
    ERROR_IPSEC_IKE_NEG_STATUS_EXTENDED_END = 13909,
    /// <summary>The SPI in the packet does not match a valid IPsec SA.</summary>
    ERROR_IPSEC_BAD_SPI = 13910,
    /// <summary>Packet was received on an IPsec SA whose lifetime has expired.</summary>
    ERROR_IPSEC_SA_LIFETIME_EXPIRED = 13911,
    /// <summary>Packet was received on an IPsec SA that does not match the packet characteristics.</summary>
    ERROR_IPSEC_WRONG_SA = 13912,
    /// <summary>Packet sequence number replay check failed.</summary>
    ERROR_IPSEC_REPLAY_CHECK_FAILED = 13913,
    /// <summary>IPsec header and/or trailer in the packet is invalid.</summary>
    ERROR_IPSEC_INVALID_PACKET = 13914,
    /// <summary>IPsec integrity check failed.</summary>
    ERROR_IPSEC_INTEGRITY_CHECK_FAILED = 13915,
    /// <summary>IPsec dropped a clear text packet.</summary>
    ERROR_IPSEC_CLEAR_TEXT_DROP = 13916,
    /// <summary>IPsec dropped an incoming ESP packet in authenticated firewall mode. This drop is benign.</summary>
    ERROR_IPSEC_AUTH_FIREWALL_DROP = 13917,
    /// <summary>IPsec dropped a packet due to DoS throttling.</summary>
    ERROR_IPSEC_THROTTLE_DROP = 13918,
    /// <summary>IPsec DoS Protection matched an explicit block rule.</summary>
    ERROR_IPSEC_DOSP_BLOCK = 13925,
    /// <summary>IPsec DoS Protection received an IPsec specific multicast packet which is not allowed.</summary>
    ERROR_IPSEC_DOSP_RECEIVED_MULTICAST = 13926,
    /// <summary>IPsec DoS Protection received an incorrectly formatted packet.</summary>
    ERROR_IPSEC_DOSP_INVALID_PACKET = 13927,
    /// <summary>IPsec DoS Protection failed to look up state.</summary>
    ERROR_IPSEC_DOSP_STATE_LOOKUP_FAILED = 13928,
    /// <summary>IPsec DoS Protection failed to create state because the maximum number of entries allowed by policy has been reached.</summary>
    ERROR_IPSEC_DOSP_MAX_ENTRIES = 13929,
    /// <summary>IPsec DoS Protection received an IPsec negotiation packet for a keying module which is not allowed by policy.</summary>
    ERROR_IPSEC_DOSP_KEYMOD_NOT_ALLOWED = 13930,
    /// <summary>IPsec DoS Protection has not been enabled.</summary>
    ERROR_IPSEC_DOSP_NOT_INSTALLED = 13931,
    /// <summary>IPsec DoS Protection failed to create a per internal IP rate limit queue because the maximum number of queues allowed by policy has been reached.</summary>
    ERROR_IPSEC_DOSP_MAX_PER_IP_RATELIMIT_QUEUES = 13932,
    /// <summary>The requested section was not present in the activation context.</summary>
    ERROR_SXS_SECTION_NOT_FOUND = 14000,
    /// <summary>The application has failed to start because its side-by-side configuration is incorrect. Please see the application event log or use the command-line sxstrace.exe tool for more detail.</summary>
    ERROR_SXS_CANT_GEN_ACTCTX = 14001,
    /// <summary>The application binding data format is invalid.</summary>
    ERROR_SXS_INVALID_ACTCTXDATA_FORMAT = 14002,
    /// <summary>The referenced assembly is not installed on your system.</summary>
    ERROR_SXS_ASSEMBLY_NOT_FOUND = 14003,
    /// <summary>The manifest file does not begin with the required tag and format information.</summary>
    ERROR_SXS_MANIFEST_FORMAT_ERROR = 14004,
    /// <summary>The manifest file contains one or more syntax errors.</summary>
    ERROR_SXS_MANIFEST_PARSE_ERROR = 14005,
    /// <summary>The application attempted to activate a disabled activation context.</summary>
    ERROR_SXS_ACTIVATION_CONTEXT_DISABLED = 14006,
    /// <summary>The requested lookup key was not found in any active activation context.</summary>
    ERROR_SXS_KEY_NOT_FOUND = 14007,
    /// <summary>A component version required by the application conflicts with another component version already active.</summary>
    ERROR_SXS_VERSION_CONFLICT = 14008,
    /// <summary>The type requested activation context section does not match the query API used.</summary>
    ERROR_SXS_WRONG_SECTION_TYPE = 14009,
    /// <summary>Lack of system resources has required isolated activation to be disabled for the current thread of execution.</summary>
    ERROR_SXS_THREAD_QUERIES_DISABLED = 14010,
    /// <summary>An attempt to set the process default activation context failed because the process default activation context was already set.</summary>
    ERROR_SXS_PROCESS_DEFAULT_ALREADY_SET = 14011,
    /// <summary>The encoding group identifier specified is not recognized.</summary>
    ERROR_SXS_UNKNOWN_ENCODING_GROUP = 14012,
    /// <summary>The encoding requested is not recognized.</summary>
    ERROR_SXS_UNKNOWN_ENCODING = 14013,
    /// <summary>The manifest contains a reference to an invalid URI.</summary>
    ERROR_SXS_INVALID_XML_NAMESPACE_URI = 14014,
    /// <summary>The application manifest contains a reference to a dependent assembly which is not installed</summary>
    ERROR_SXS_ROOT_MANIFEST_DEPENDENCY_NOT_INSTALLED = 14015,
    /// <summary>The manifest for an assembly used by the application has a reference to a dependent assembly which is not installed</summary>
    ERROR_SXS_LEAF_MANIFEST_DEPENDENCY_NOT_INSTALLED = 14016,
    /// <summary>The manifest contains an attribute for the assembly identity which is not valid.</summary>
    ERROR_SXS_INVALID_ASSEMBLY_IDENTITY_ATTRIBUTE = 14017,
    /// <summary>The manifest is missing the required default namespace specification on the assembly element.</summary>
    ERROR_SXS_MANIFEST_MISSING_REQUIRED_DEFAULT_NAMESPACE = 14018,
    /// <summary>The manifest has a default namespace specified on the assembly element but its value is not &quot;urn:schemas-microsoft-com:asm.v1&quot;.</summary>
    ERROR_SXS_MANIFEST_INVALID_REQUIRED_DEFAULT_NAMESPACE = 14019,
    /// <summary>The private manifest probed has crossed a path with an unsupported reparse point.</summary>
    ERROR_SXS_PRIVATE_MANIFEST_CROSS_PATH_WITH_REPARSE_POINT = 14020,
    /// <summary>Two or more components referenced directly or indirectly by the application manifest have files by the same name.</summary>
    ERROR_SXS_DUPLICATE_DLL_NAME = 14021,
    /// <summary>Two or more components referenced directly or indirectly by the application manifest have window classes with the same name.</summary>
    ERROR_SXS_DUPLICATE_WINDOWCLASS_NAME = 14022,
    /// <summary>Two or more components referenced directly or indirectly by the application manifest have the same COM server CLSIDs.</summary>
    ERROR_SXS_DUPLICATE_CLSID = 14023,
    /// <summary>Two or more components referenced directly or indirectly by the application manifest have proxies for the same COM interface IIDs.</summary>
    ERROR_SXS_DUPLICATE_IID = 14024,
    /// <summary>Two or more components referenced directly or indirectly by the application manifest have the same COM type library TLBIDs.</summary>
    ERROR_SXS_DUPLICATE_TLBID = 14025,
    /// <summary>Two or more components referenced directly or indirectly by the application manifest have the same COM ProgIDs.</summary>
    ERROR_SXS_DUPLICATE_PROGID = 14026,
    /// <summary>Two or more components referenced directly or indirectly by the application manifest are different versions of the same component which is not permitted.</summary>
    ERROR_SXS_DUPLICATE_ASSEMBLY_NAME = 14027,
    /// <summary>A component&#39;s file does not match the verification information present in the component manifest.</summary>
    ERROR_SXS_FILE_HASH_MISMATCH = 14028,
    /// <summary>The policy manifest contains one or more syntax errors.</summary>
    ERROR_SXS_POLICY_PARSE_ERROR = 14029,
    /// <summary>Manifest Parse Error : A string literal was expected, but no opening quote character was found.</summary>
    ERROR_SXS_XML_E_MISSINGQUOTE = 14030,
    /// <summary>Manifest Parse Error : Incorrect syntax was used in a comment.</summary>
    ERROR_SXS_XML_E_COMMENTSYNTAX = 14031,
    /// <summary>Manifest Parse Error : A name was started with an invalid character.</summary>
    ERROR_SXS_XML_E_BADSTARTNAMECHAR = 14032,
    /// <summary>Manifest Parse Error : A name contained an invalid character.</summary>
    ERROR_SXS_XML_E_BADNAMECHAR = 14033,
    /// <summary>Manifest Parse Error : A string literal contained an invalid character.</summary>
    ERROR_SXS_XML_E_BADCHARINSTRING = 14034,
    /// <summary>Manifest Parse Error : Invalid syntax for an xml declaration.</summary>
    ERROR_SXS_XML_E_XMLDECLSYNTAX = 14035,
    /// <summary>Manifest Parse Error : An Invalid character was found in text content.</summary>
    ERROR_SXS_XML_E_BADCHARDATA = 14036,
    /// <summary>Manifest Parse Error : Required white space was missing.</summary>
    ERROR_SXS_XML_E_MISSINGWHITESPACE = 14037,
    /// <summary>Manifest Parse Error : The character &#39;&gt;&#39; was expected.</summary>
    ERROR_SXS_XML_E_EXPECTINGTAGEND = 14038,
    /// <summary>Manifest Parse Error : A semi colon character was expected.</summary>
    ERROR_SXS_XML_E_MISSINGSEMICOLON = 14039,
    /// <summary>Manifest Parse Error : Unbalanced parentheses.</summary>
    ERROR_SXS_XML_E_UNBALANCEDPAREN = 14040,
    /// <summary>Manifest Parse Error : Internal error.</summary>
    ERROR_SXS_XML_E_INTERNALERROR = 14041,
    /// <summary>Manifest Parse Error : Whitespace is not allowed at this location.</summary>
    ERROR_SXS_XML_E_UNEXPECTED_WHITESPACE = 14042,
    /// <summary>Manifest Parse Error : End of file reached in invalid state for current encoding.</summary>
    ERROR_SXS_XML_E_INCOMPLETE_ENCODING = 14043,
    /// <summary>Manifest Parse Error : Missing parenthesis.</summary>
    ERROR_SXS_XML_E_MISSING_PAREN = 14044,
    /// <summary>Manifest Parse Error : A single or double closing quote character (\&#39; or \&quot;) is missing.</summary>
    ERROR_SXS_XML_E_EXPECTINGCLOSEQUOTE = 14045,
    /// <summary>Manifest Parse Error : Multiple colons are not allowed in a name.</summary>
    ERROR_SXS_XML_E_MULTIPLE_COLONS = 14046,
    /// <summary>Manifest Parse Error : Invalid character for decimal digit.</summary>
    ERROR_SXS_XML_E_INVALID_DECIMAL = 14047,
    /// <summary>Manifest Parse Error : Invalid character for hexadecimal digit.</summary>
    ERROR_SXS_XML_E_INVALID_HEXIDECIMAL = 14048,
    /// <summary>Manifest Parse Error : Invalid unicode character value for this platform.</summary>
    ERROR_SXS_XML_E_INVALID_UNICODE = 14049,
    /// <summary>Manifest Parse Error : Expecting whitespace or &#39;?&#39;.</summary>
    ERROR_SXS_XML_E_WHITESPACEORQUESTIONMARK = 14050,
    /// <summary>Manifest Parse Error : End tag was not expected at this location.</summary>
    ERROR_SXS_XML_E_UNEXPECTEDENDTAG = 14051,
    /// <summary>Manifest Parse Error : The following tags were not closed: %1.</summary>
    ERROR_SXS_XML_E_UNCLOSEDTAG = 14052,
    /// <summary>Manifest Parse Error : Duplicate attribute.</summary>
    ERROR_SXS_XML_E_DUPLICATEATTRIBUTE = 14053,
    /// <summary>Manifest Parse Error : Only one top level element is allowed in an XML document.</summary>
    ERROR_SXS_XML_E_MULTIPLEROOTS = 14054,
    /// <summary>Manifest Parse Error : Invalid at the top level of the document.</summary>
    ERROR_SXS_XML_E_INVALIDATROOTLEVEL = 14055,
    /// <summary>Manifest Parse Error : Invalid xml declaration.</summary>
    ERROR_SXS_XML_E_BADXMLDECL = 14056,
    /// <summary>Manifest Parse Error : XML document must have a top level element.</summary>
    ERROR_SXS_XML_E_MISSINGROOT = 14057,
    /// <summary>Manifest Parse Error : Unexpected end of file.</summary>
    ERROR_SXS_XML_E_UNEXPECTEDEOF = 14058,
    /// <summary>Manifest Parse Error : Parameter entities cannot be used inside markup declarations in an internal subset.</summary>
    ERROR_SXS_XML_E_BADPEREFINSUBSET = 14059,
    /// <summary>Manifest Parse Error : Element was not closed.</summary>
    ERROR_SXS_XML_E_UNCLOSEDSTARTTAG = 14060,
    /// <summary>Manifest Parse Error : End element was missing the character &#39;&gt;&#39;.</summary>
    ERROR_SXS_XML_E_UNCLOSEDENDTAG = 14061,
    /// <summary>Manifest Parse Error : A string literal was not closed.</summary>
    ERROR_SXS_XML_E_UNCLOSEDSTRING = 14062,
    /// <summary>Manifest Parse Error : A comment was not closed.</summary>
    ERROR_SXS_XML_E_UNCLOSEDCOMMENT = 14063,
    /// <summary>Manifest Parse Error : A declaration was not closed.</summary>
    ERROR_SXS_XML_E_UNCLOSEDDECL = 14064,
    /// <summary>Manifest Parse Error : A CDATA section was not closed.</summary>
    ERROR_SXS_XML_E_UNCLOSEDCDATA = 14065,
    /// <summary>Manifest Parse Error : The namespace prefix is not allowed to start with the reserved string &quot;xml&quot;.</summary>
    ERROR_SXS_XML_E_RESERVEDNAMESPACE = 14066,
    /// <summary>Manifest Parse Error : System does not support the specified encoding.</summary>
    ERROR_SXS_XML_E_INVALIDENCODING = 14067,
    /// <summary>Manifest Parse Error : Switch from current encoding to specified encoding not supported.</summary>
    ERROR_SXS_XML_E_INVALIDSWITCH = 14068,
    /// <summary>Manifest Parse Error : The name &#39;xml&#39; is reserved and must be lower case.</summary>
    ERROR_SXS_XML_E_BADXMLCASE = 14069,
    /// <summary>Manifest Parse Error : The standalone attribute must have the value &#39;yes&#39; or &#39;no&#39;.</summary>
    ERROR_SXS_XML_E_INVALID_STANDALONE = 14070,
    /// <summary>Manifest Parse Error : The standalone attribute cannot be used in external entities.</summary>
    ERROR_SXS_XML_E_UNEXPECTED_STANDALONE = 14071,
    /// <summary>Manifest Parse Error : Invalid version number.</summary>
    ERROR_SXS_XML_E_INVALID_VERSION = 14072,
    /// <summary>Manifest Parse Error : Missing equals sign between attribute and attribute value.</summary>
    ERROR_SXS_XML_E_MISSINGEQUALS = 14073,
    /// <summary>Assembly Protection Error : Unable to recover the specified assembly.</summary>
    ERROR_SXS_PROTECTION_RECOVERY_FAILED = 14074,
    /// <summary>Assembly Protection Error : The public key for an assembly was too short to be allowed.</summary>
    ERROR_SXS_PROTECTION_PUBLIC_KEY_TOO_SHORT = 14075,
    /// <summary>Assembly Protection Error : The catalog for an assembly is not valid, or does not match the assembly&#39;s manifest.</summary>
    ERROR_SXS_PROTECTION_CATALOG_NOT_VALID = 14076,
    /// <summary>An HRESULT could not be translated to a corresponding Win32 error code.</summary>
    ERROR_SXS_UNTRANSLATABLE_HRESULT = 14077,
    /// <summary>Assembly Protection Error : The catalog for an assembly is missing.</summary>
    ERROR_SXS_PROTECTION_CATALOG_FILE_MISSING = 14078,
    /// <summary>The supplied assembly identity is missing one or more attributes which must be present in this context.</summary>
    ERROR_SXS_MISSING_ASSEMBLY_IDENTITY_ATTRIBUTE = 14079,
    /// <summary>The supplied assembly identity has one or more attribute names that contain characters not permitted in XML names.</summary>
    ERROR_SXS_INVALID_ASSEMBLY_IDENTITY_ATTRIBUTE_NAME = 14080,
    /// <summary>The referenced assembly could not be found.</summary>
    ERROR_SXS_ASSEMBLY_MISSING = 14081,
    /// <summary>The activation context activation stack for the running thread of execution is corrupt.</summary>
    ERROR_SXS_CORRUPT_ACTIVATION_STACK = 14082,
    /// <summary>The application isolation metadata for this process or thread has become corrupt.</summary>
    ERROR_SXS_CORRUPTION = 14083,
    /// <summary>The activation context being deactivated is not the most recently activated one.</summary>
    ERROR_SXS_EARLY_DEACTIVATION = 14084,
    /// <summary>The activation context being deactivated is not active for the current thread of execution.</summary>
    ERROR_SXS_INVALID_DEACTIVATION = 14085,
    /// <summary>The activation context being deactivated has already been deactivated.</summary>
    ERROR_SXS_MULTIPLE_DEACTIVATION = 14086,
    /// <summary>A component used by the isolation facility has requested to terminate the process.</summary>
    ERROR_SXS_PROCESS_TERMINATION_REQUESTED = 14087,
    /// <summary>A kernel mode component is releasing a reference on an activation context.</summary>
    ERROR_SXS_RELEASE_ACTIVATION_CONTEXT = 14088,
    /// <summary>The activation context of system default assembly could not be generated.</summary>
    ERROR_SXS_SYSTEM_DEFAULT_ACTIVATION_CONTEXT_EMPTY = 14089,
    /// <summary>The value of an attribute in an identity is not within the legal range.</summary>
    ERROR_SXS_INVALID_IDENTITY_ATTRIBUTE_VALUE = 14090,
    /// <summary>The name of an attribute in an identity is not within the legal range.</summary>
    ERROR_SXS_INVALID_IDENTITY_ATTRIBUTE_NAME = 14091,
    /// <summary>An identity contains two definitions for the same attribute.</summary>
    ERROR_SXS_IDENTITY_DUPLICATE_ATTRIBUTE = 14092,
    /// <summary>The identity string is malformed. This may be due to a trailing comma, more than two unnamed attributes, missing attribute name or missing attribute value.</summary>
    ERROR_SXS_IDENTITY_PARSE_ERROR = 14093,
    /// <summary>A string containing localized substitutable content was malformed. Either a dollar sign ($) was followed by something other than a left parenthesis or another dollar sign or an substitution&#39;s right parenthesis was not found.</summary>
    ERROR_MALFORMED_SUBSTITUTION_STRING = 14094,
    /// <summary>The public key token does not correspond to the public key specified.</summary>
    ERROR_SXS_INCORRECT_PUBLIC_KEY_TOKEN = 14095,
    /// <summary>A substitution string had no mapping.</summary>
    ERROR_UNMAPPED_SUBSTITUTION_STRING = 14096,
    /// <summary>The component must be locked before making the request.</summary>
    ERROR_SXS_ASSEMBLY_NOT_LOCKED = 14097,
    /// <summary>The component store has been corrupted.</summary>
    ERROR_SXS_COMPONENT_STORE_CORRUPT = 14098,
    /// <summary>An advanced installer failed during setup or servicing.</summary>
    ERROR_ADVANCED_INSTALLER_FAILED = 14099,
    /// <summary>The character encoding in the XML declaration did not match the encoding used in the document.</summary>
    ERROR_XML_ENCODING_MISMATCH = 14100,
    /// <summary>The identities of the manifests are identical but their contents are different.</summary>
    ERROR_SXS_MANIFEST_IDENTITY_SAME_BUT_CONTENTS_DIFFERENT = 14101,
    /// <summary>The component identities are different.</summary>
    ERROR_SXS_IDENTITIES_DIFFERENT = 14102,
    /// <summary>The assembly is not a deployment.</summary>
    ERROR_SXS_ASSEMBLY_IS_NOT_A_DEPLOYMENT = 14103,
    /// <summary>The file is not a part of the assembly.</summary>
    ERROR_SXS_FILE_NOT_PART_OF_ASSEMBLY = 14104,
    /// <summary>The size of the manifest exceeds the maximum allowed.</summary>
    ERROR_SXS_MANIFEST_TOO_BIG = 14105,
    /// <summary>The setting is not registered.</summary>
    ERROR_SXS_SETTING_NOT_REGISTERED = 14106,
    /// <summary>One or more required members of the transaction are not present.</summary>
    ERROR_SXS_TRANSACTION_CLOSURE_INCOMPLETE = 14107,
    /// <summary>The SMI primitive installer failed during setup or servicing.</summary>
    ERROR_SMI_PRIMITIVE_INSTALLER_FAILED = 14108,
    /// <summary>A generic command executable returned a result that indicates failure.</summary>
    ERROR_GENERIC_COMMAND_FAILED = 14109,
    /// <summary>A component is missing file verification information in its manifest.</summary>
    ERROR_SXS_FILE_HASH_MISSING = 14110,
    /// <summary>The specified channel path is invalid.</summary>
    ERROR_EVT_INVALID_CHANNEL_PATH = 15000,
    /// <summary>The specified query is invalid.</summary>
    ERROR_EVT_INVALID_QUERY = 15001,
    /// <summary>The publisher metadata cannot be found in the resource.</summary>
    ERROR_EVT_PUBLISHER_METADATA_NOT_FOUND = 15002,
    /// <summary>The template for an event definition cannot be found in the resource (error = %1).</summary>
    ERROR_EVT_EVENT_TEMPLATE_NOT_FOUND = 15003,
    /// <summary>The specified publisher name is invalid.</summary>
    ERROR_EVT_INVALID_PUBLISHER_NAME = 15004,
    /// <summary>The event data raised by the publisher is not compatible with the event template definition in the publisher&#39;s manifest</summary>
    ERROR_EVT_INVALID_EVENT_DATA = 15005,
    /// <summary>The specified channel could not be found. Check channel configuration.</summary>
    ERROR_EVT_CHANNEL_NOT_FOUND = 15007,
    /// <summary>The specified xml text was not well-formed. See Extended Error for more details.</summary>
    ERROR_EVT_MALFORMED_XML_TEXT = 15008,
    /// <summary>The caller is trying to subscribe to a direct channel which is not allowed. The events for a direct channel go directly to a logfile and cannot be subscribed to.</summary>
    ERROR_EVT_SUBSCRIPTION_TO_DIRECT_CHANNEL = 15009,
    /// <summary>Configuration error.</summary>
    ERROR_EVT_CONFIGURATION_ERROR = 15010,
    /// <summary>The query result is stale / invalid. This may be due to the log being cleared or rolling over after the query result was created. Users should handle this code by releasing the query result object and reissuing the query.</summary>
    ERROR_EVT_QUERY_RESULT_STALE = 15011,
    /// <summary>Query result is currently at an invalid position.</summary>
    ERROR_EVT_QUERY_RESULT_INVALID_POSITION = 15012,
    /// <summary>Registered MSXML doesn&#39;t support validation.</summary>
    ERROR_EVT_NON_VALIDATING_MSXML = 15013,
    /// <summary>An expression can only be followed by a change of scope operation if it itself evaluates to a node set and is not already part of some other change of scope operation.</summary>
    ERROR_EVT_FILTER_ALREADYSCOPED = 15014,
    /// <summary>Can&#39;t perform a step operation from a term that does not represent an element set.</summary>
    ERROR_EVT_FILTER_NOTELTSET = 15015,
    /// <summary>Left hand side arguments to binary operators must be either attributes, nodes or variables and right hand side arguments must be constants.</summary>
    ERROR_EVT_FILTER_INVARG = 15016,
    /// <summary>A step operation must involve either a node test or, in the case of a predicate, an algebraic expression against which to test each node in the node set identified by the preceeding node set can be evaluated.</summary>
    ERROR_EVT_FILTER_INVTEST = 15017,
    /// <summary>This data type is currently unsupported.</summary>
    ERROR_EVT_FILTER_INVTYPE = 15018,
    /// <summary>A syntax error occurred at position %1!d!</summary>
    ERROR_EVT_FILTER_PARSEERR = 15019,
    /// <summary>This operator is unsupported by this implementation of the filter.</summary>
    ERROR_EVT_FILTER_UNSUPPORTEDOP = 15020,
    /// <summary>The token encountered was unexpected.</summary>
    ERROR_EVT_FILTER_UNEXPECTEDTOKEN = 15021,
    /// <summary>The requested operation cannot be performed over an enabled direct channel. The channel must first be disabled before performing the requested operation.</summary>
    ERROR_EVT_INVALID_OPERATION_OVER_ENABLED_DIRECT_CHANNEL = 15022,
    /// <summary>Channel property %1!s! contains invalid value. The value has invalid type, is outside of valid range, can&#39;t be updated or is not supported by this type of channel.</summary>
    ERROR_EVT_INVALID_CHANNEL_PROPERTY_VALUE = 15023,
    /// <summary>Publisher property %1!s! contains invalid value. The value has invalid type, is outside of valid range, can&#39;t be updated or is not supported by this type of publisher.</summary>
    ERROR_EVT_INVALID_PUBLISHER_PROPERTY_VALUE = 15024,
    /// <summary>The channel fails to activate.</summary>
    ERROR_EVT_CHANNEL_CANNOT_ACTIVATE = 15025,
    /// <summary>The xpath expression exceeded supported complexity. Please symplify it or split it into two or more simple expressions.</summary>
    ERROR_EVT_FILTER_TOO_COMPLEX = 15026,
    /// <summary>the message resource is present but the message is not found in the string/message table</summary>
    ERROR_EVT_MESSAGE_NOT_FOUND = 15027,
    /// <summary>The message id for the desired message could not be found.</summary>
    ERROR_EVT_MESSAGE_ID_NOT_FOUND = 15028,
    /// <summary>The substitution string for insert index (%1) could not be found.</summary>
    ERROR_EVT_UNRESOLVED_VALUE_INSERT = 15029,
    /// <summary>The description string for parameter reference (%1) could not be found.</summary>
    ERROR_EVT_UNRESOLVED_PARAMETER_INSERT = 15030,
    /// <summary>The maximum number of replacements has been reached.</summary>
    ERROR_EVT_MAX_INSERTS_REACHED = 15031,
    /// <summary>The event definition could not be found for event id (%1).</summary>
    ERROR_EVT_EVENT_DEFINITION_NOT_FOUND = 15032,
    /// <summary>The locale specific resource for the desired message is not present.</summary>
    ERROR_EVT_MESSAGE_LOCALE_NOT_FOUND = 15033,
    /// <summary>The resource is too old to be compatible.</summary>
    ERROR_EVT_VERSION_TOO_OLD = 15034,
    /// <summary>The resource is too new to be compatible.</summary>
    ERROR_EVT_VERSION_TOO_NEW = 15035,
    /// <summary>The channel at index %1!d! of the query can&#39;t be opened.</summary>
    ERROR_EVT_CANNOT_OPEN_CHANNEL_OF_QUERY = 15036,
    /// <summary>The publisher has been disabled and its resource is not avaiable. This usually occurs when the publisher is in the process of being uninstalled or upgraded.</summary>
    ERROR_EVT_PUBLISHER_DISABLED = 15037,
    /// <summary>Attempted to create a numeric type that is outside of its valid range.</summary>
    ERROR_EVT_FILTER_OUT_OF_RANGE = 15038,
    /// <summary>The subscription fails to activate.</summary>
    ERROR_EC_SUBSCRIPTION_CANNOT_ACTIVATE = 15080,
    /// <summary>The log of the subscription is in disabled state, and can not be used to forward events to. The log must first be enabled before the subscription can be activated.</summary>
    ERROR_EC_LOG_DISABLED = 15081,
    /// <summary>When forwarding events from local machine to itself, the query of the subscription can&#39;t contain target log of the subscription.</summary>
    ERROR_EC_CIRCULAR_FORWARDING = 15082,
    /// <summary>The credential store that is used to save credentials is full.</summary>
    ERROR_EC_CREDSTORE_FULL = 15083,
    /// <summary>The credential used by this subscription can&#39;t be found in credential store.</summary>
    ERROR_EC_CRED_NOT_FOUND = 15084,
    /// <summary>No active channel is found for the query.</summary>
    ERROR_EC_NO_ACTIVE_CHANNEL = 15085,
    /// <summary>The resource loader failed to find MUI file.</summary>
    ERROR_MUI_FILE_NOT_FOUND = 15100,
    /// <summary>The resource loader failed to load MUI file because the file fail to pass validation.</summary>
    ERROR_MUI_INVALID_FILE = 15101,
    /// <summary>The RC Manifest is corrupted with garbage data or unsupported version or missing required item.</summary>
    ERROR_MUI_INVALID_RC_CONFIG = 15102,
    /// <summary>The RC Manifest has invalid culture name.</summary>
    ERROR_MUI_INVALID_LOCALE_NAME = 15103,
    /// <summary>The RC Manifest has invalid ultimatefallback name.</summary>
    ERROR_MUI_INVALID_ULTIMATEFALLBACK_NAME = 15104,
    /// <summary>The resource loader cache doesn&#39;t have loaded MUI entry.</summary>
    ERROR_MUI_FILE_NOT_LOADED = 15105,
    /// <summary>User stopped resource enumeration.</summary>
    ERROR_RESOURCE_ENUM_USER_STOP = 15106,
    /// <summary>UI language installation failed.</summary>
    ERROR_MUI_INTLSETTINGS_UILANG_NOT_INSTALLED = 15107,
    /// <summary>Locale installation failed.</summary>
    ERROR_MUI_INTLSETTINGS_INVALID_LOCALE_NAME = 15108,
    /// <summary>The monitor returned a DDC/CI capabilities string that did not comply with the ACCESS.bus 3.0, DDC/CI 1.1 or MCCS 2 Revision 1 specification.</summary>
    ERROR_MCA_INVALID_CAPABILITIES_STRING = 15200,
    /// <summary>The monitor&#39;s VCP Version (0xDF) VCP code returned an invalid version value.</summary>
    ERROR_MCA_INVALID_VCP_VERSION = 15201,
    /// <summary>The monitor does not comply with the MCCS specification it claims to support.</summary>
    ERROR_MCA_MONITOR_VIOLATES_MCCS_SPECIFICATION = 15202,
    /// <summary>The MCCS version in a monitor&#39;s mccs_ver capability does not match the MCCS version the monitor reports when the VCP Version (0xDF) VCP code is used.</summary>
    ERROR_MCA_MCCS_VERSION_MISMATCH = 15203,
    /// <summary>The Monitor Configuration API only works with monitors that support the MCCS 1.0 specification, MCCS 2.0 specification or the MCCS 2.0 Revision 1 specification.</summary>
    ERROR_MCA_UNSUPPORTED_MCCS_VERSION = 15204,
    /// <summary>An internal Monitor Configuration API error occurred.</summary>
    ERROR_MCA_INTERNAL_ERROR = 15205,
    /// <summary>The monitor returned an invalid monitor technology type. CRT, Plasma and LCD (TFT) are examples of monitor technology types. This error implies that the monitor violated the MCCS 2.0 or MCCS 2.0 Revision 1 specification.</summary>
    ERROR_MCA_INVALID_TECHNOLOGY_TYPE_RETURNED = 15206,
    /// <summary>The caller of SetMonitorColorTemperature specified a color temperature that the current monitor did not support. This error implies that the monitor violated the MCCS 2.0 or MCCS 2.0 Revision 1 specification.</summary>
    ERROR_MCA_UNSUPPORTED_COLOR_TEMPERATURE = 15207,
    /// <summary>The requested system device cannot be identified due to multiple indistinguishable devices potentially matching the identification criteria.</summary>
    ERROR_AMBIGUOUS_SYSTEM_DEVICE = 15250,
    /// <summary>The requested system device cannot be found.</summary>
    ERROR_SYSTEM_DEVICE_NOT_FOUND = 15299,
    /// <summary>Hash generation for the specified hash version and hash type is not enabled on the server.</summary>
    ERROR_HASH_NOT_SUPPORTED = 15300,
    /// <summary>The hash requested from the server is not available or no longer valid.</summary>
    ERROR_HASH_NOT_PRESENT = 15301,
    /// <summary>Catastrophic failure</summary>
    E_UNEXPECTED = 0x8000FFFF,
    /// <summary>Not implemented</summary>
    E_NOTIMPL = 0x80004001,
    /// <summary>Ran out of memory</summary>
    E_OUTOFMEMORY = 0x8007000E,
    /// <summary>One or more arguments are invalid</summary>
    E_INVALIDARG = 0x80070057,
    /// <summary>No such interface supported</summary>
    E_NOINTERFACE = 0x80004002,
    /// <summary>Invalid pointer</summary>
    E_POINTER = 0x80004003,
    /// <summary>Invalid handle</summary>
    E_HANDLE = 0x80070006,
    /// <summary>Operation aborted</summary>
    E_ABORT = 0x80004004,
    /// <summary>Unspecified error</summary>
    E_FAIL = 0x80004005,
    /// <summary>General access denied error</summary>
    E_ACCESSDENIED = 0x80070005,
    /// <summary>The data necessary to complete this operation is not yet available.</summary>
    E_PENDING = 0x8000000A,
    /// <summary>Thread local storage failure</summary>
    CO_E_INIT_TLS = 0x80004006,
    /// <summary>Get shared memory allocator failure</summary>
    CO_E_INIT_SHARED_ALLOCATOR = 0x80004007,
    /// <summary>Get memory allocator failure</summary>
    CO_E_INIT_MEMORY_ALLOCATOR = 0x80004008,
    /// <summary>Unable to initialize class cache</summary>
    CO_E_INIT_CLASS_CACHE = 0x80004009,
    /// <summary>Unable to initialize RPC services</summary>
    CO_E_INIT_RPC_CHANNEL = 0x8000400A,
    /// <summary>Cannot set thread local storage channel control</summary>
    CO_E_INIT_TLS_SET_CHANNEL_CONTROL = 0x8000400B,
    /// <summary>Could not allocate thread local storage channel control</summary>
    CO_E_INIT_TLS_CHANNEL_CONTROL = 0x8000400C,
    /// <summary>The user supplied memory allocator is unacceptable</summary>
    CO_E_INIT_UNACCEPTED_USER_ALLOCATOR = 0x8000400D,
    /// <summary>The OLE service mutex already exists</summary>
    CO_E_INIT_SCM_MUTEX_EXISTS = 0x8000400E,
    /// <summary>The OLE service file mapping already exists</summary>
    CO_E_INIT_SCM_FILE_MAPPING_EXISTS = 0x8000400F,
    /// <summary>Unable to map view of file for OLE service</summary>
    CO_E_INIT_SCM_MAP_VIEW_OF_FILE = 0x80004010,
    /// <summary>Failure attempting to launch OLE service</summary>
    CO_E_INIT_SCM_EXEC_FAILURE = 0x80004011,
    /// <summary>There was an attempt to call CoInitialize a second time while single threaded</summary>
    CO_E_INIT_ONLY_SINGLE_THREADED = 0x80004012,
    /// <summary>A Remote activation was necessary but was not allowed</summary>
    CO_E_CANT_REMOTE = 0x80004013,
    /// <summary>A Remote activation was necessary but the server name provided was invalid</summary>
    CO_E_BAD_SERVER_NAME = 0x80004014,
    /// <summary>The class is configured to run as a security id different from the caller</summary>
    CO_E_WRONG_SERVER_IDENTITY = 0x80004015,
    /// <summary>Use of Ole1 services requiring DDE windows is disabled</summary>
    CO_E_OLE1DDE_DISABLED = 0x80004016,
    /// <summary>A RunAs specification must be &lt;domain name&gt;\&lt;user name&gt; or simply &lt;user name&gt;</summary>
    CO_E_RUNAS_SYNTAX = 0x80004017,
    /// <summary>The server process could not be started. The pathname may be incorrect.</summary>
    CO_E_CREATEPROCESS_FAILURE = 0x80004018,
    /// <summary>The server process could not be started as the configured identity. The pathname may be incorrect or unavailable.</summary>
    CO_E_RUNAS_CREATEPROCESS_FAILURE = 0x80004019,
    /// <summary>The server process could not be started because the configured identity is incorrect. Check the username and password.</summary>
    CO_E_RUNAS_LOGON_FAILURE = 0x8000401A,
    /// <summary>The client is not allowed to launch this server.</summary>
    CO_E_LAUNCH_PERMSSION_DENIED = 0x8000401B,
    /// <summary>The service providing this server could not be started.</summary>
    CO_E_START_SERVICE_FAILURE = 0x8000401C,
    /// <summary>This computer was unable to communicate with the computer providing the server.</summary>
    CO_E_REMOTE_COMMUNICATION_FAILURE = 0x8000401D,
    /// <summary>The server did not respond after being launched.</summary>
    CO_E_SERVER_START_TIMEOUT = 0x8000401E,
    /// <summary>The registration information for this server is inconsistent or incomplete.</summary>
    CO_E_CLSREG_INCONSISTENT = 0x8000401F,
    /// <summary>The registration information for this interface is inconsistent or incomplete.</summary>
    CO_E_IIDREG_INCONSISTENT = 0x80004020,
    /// <summary>The operation attempted is not supported.</summary>
    CO_E_NOT_SUPPORTED = 0x80004021,
    /// <summary>A dll must be loaded.</summary>
    CO_E_RELOAD_DLL = 0x80004022,
    /// <summary>A Microsoft Software Installer error was encountered.</summary>
    CO_E_MSI_ERROR = 0x80004023,
    /// <summary>The specified activation could not occur in the client context as specified.</summary>
    CO_E_ATTEMPT_TO_CREATE_OUTSIDE_CLIENT_CONTEXT = 0x80004024,
    /// <summary>Activations on the server are paused.</summary>
    CO_E_SERVER_PAUSED = 0x80004025,
    /// <summary>Activations on the server are not paused.</summary>
    CO_E_SERVER_NOT_PAUSED = 0x80004026,
    /// <summary>The component or application containing the component has been disabled.</summary>
    CO_E_CLASS_DISABLED = 0x80004027,
    /// <summary>The common language runtime is not available</summary>
    CO_E_CLRNOTAVAILABLE = 0x80004028,
    /// <summary>The thread-pool rejected the submitted asynchronous work.</summary>
    CO_E_ASYNC_WORK_REJECTED = 0x80004029,
    /// <summary>The server started, but did not finish initializing in a timely fashion.</summary>
    CO_E_SERVER_INIT_TIMEOUT = 0x8000402A,
    /// <summary>Unable to complete the call since there is no COM+ security context inside IObjectControl.Activate.</summary>
    CO_E_NO_SECCTX_IN_ACTIVATE = 0x8000402B,
    /// <summary>The provided tracker configuration is invalid</summary>
    CO_E_TRACKER_CONFIG = 0x80004030,
    /// <summary>The provided thread pool configuration is invalid</summary>
    CO_E_THREADPOOL_CONFIG = 0x80004031,
    /// <summary>The provided side-by-side configuration is invalid</summary>
    CO_E_SXS_CONFIG = 0x80004032,
    /// <summary>The server principal name (SPN) obtained during security negotiation is malformed.</summary>
    CO_E_MALFORMED_SPN = 0x80004033,
    /// <summary>Invalid OLEVERB structure</summary>
    OLE_E_OLEVERB = 0x80040000,
    /// <summary>Invalid advise flags</summary>
    OLE_E_ADVF = 0x80040001,
    /// <summary>Can&#39;t enumerate any more, because the associated data is missing</summary>
    OLE_E_ENUM_NOMORE = 0x80040002,
    /// <summary>This implementation doesn&#39;t take advises</summary>
    OLE_E_ADVISENOTSUPPORTED = 0x80040003,
    /// <summary>There is no connection for this connection ID</summary>
    OLE_E_NOCONNECTION = 0x80040004,
    /// <summary>Need to run the object to perform this operation</summary>
    OLE_E_NOTRUNNING = 0x80040005,
    /// <summary>There is no cache to operate on</summary>
    OLE_E_NOCACHE = 0x80040006,
    /// <summary>Uninitialized object</summary>
    OLE_E_BLANK = 0x80040007,
    /// <summary>Linked object&#39;s source class has changed</summary>
    OLE_E_CLASSDIFF = 0x80040008,
    /// <summary>Not able to get the moniker of the object</summary>
    OLE_E_CANT_GETMONIKER = 0x80040009,
    /// <summary>Not able to bind to the source</summary>
    OLE_E_CANT_BINDTOSOURCE = 0x8004000A,
    /// <summary>Object is static; operation not allowed</summary>
    OLE_E_STATIC = 0x8004000B,
    /// <summary>User canceled out of save dialog</summary>
    OLE_E_PROMPTSAVECANCELLED = 0x8004000C,
    /// <summary>Invalid rectangle</summary>
    OLE_E_INVALIDRECT = 0x8004000D,
    /// <summary>compobj.dll is too old for the ole2.dll initialized</summary>
    OLE_E_WRONGCOMPOBJ = 0x8004000E,
    /// <summary>Invalid window handle</summary>
    OLE_E_INVALIDHWND = 0x8004000F,
    /// <summary>Object is not in any of the inplace active states</summary>
    OLE_E_NOT_INPLACEACTIVE = 0x80040010,
    /// <summary>Not able to convert object</summary>
    OLE_E_CANTCONVERT = 0x80040011,
    /// <summary>Not able to perform the operation because object is not given storage yet</summary>
    OLE_E_NOSTORAGE = 0x80040012,
    /// <summary>Invalid FORMATETC structure</summary>
    DV_E_FORMATETC = 0x80040064,
    /// <summary>Invalid DVTARGETDEVICE structure</summary>
    DV_E_DVTARGETDEVICE = 0x80040065,
    /// <summary>Invalid STDGMEDIUM structure</summary>
    DV_E_STGMEDIUM = 0x80040066,
    /// <summary>Invalid STATDATA structure</summary>
    DV_E_STATDATA = 0x80040067,
    /// <summary>Invalid lindex</summary>
    DV_E_LINDEX = 0x80040068,
    /// <summary>Invalid tymed</summary>
    DV_E_TYMED = 0x80040069,
    /// <summary>Invalid clipboard format</summary>
    DV_E_CLIPFORMAT = 0x8004006A,
    /// <summary>Invalid aspect(s)</summary>
    DV_E_DVASPECT = 0x8004006B,
    /// <summary>tdSize parameter of the DVTARGETDEVICE structure is invalid</summary>
    DV_E_DVTARGETDEVICE_SIZE = 0x8004006C,
    /// <summary>Object doesn&#39;t support IViewObject interface</summary>
    DV_E_NOIVIEWOBJECT = 0x8004006D,
    /// <summary>Trying to revoke a drop target that has not been registered</summary>
    DRAGDROP_E_NOTREGISTERED = 0x80040100,
    /// <summary>This window has already been registered as a drop target</summary>
    DRAGDROP_E_ALREADYREGISTERED = 0x80040101,
    /// <summary>Invalid window handle</summary>
    DRAGDROP_E_INVALIDHWND = 0x80040102,
    /// <summary>Class does not support aggregation (or class object is remote)</summary>
    CLASS_E_NOAGGREGATION = 0x80040110,
    /// <summary>ClassFactory cannot supply requested class</summary>
    CLASS_E_CLASSNOTAVAILABLE = 0x80040111,
    /// <summary>Class is not licensed for use</summary>
    CLASS_E_NOTLICENSED = 0x80040112,
    /// <summary>Error drawing view</summary>
    VIEW_E_DRAW = 0x80040140,
    /// <summary>Could not read key from registry</summary>
    REGDB_E_READREGDB = 0x80040150,
    /// <summary>Could not write key to registry</summary>
    REGDB_E_WRITEREGDB = 0x80040151,
    /// <summary>Could not find the key in the registry</summary>
    REGDB_E_KEYMISSING = 0x80040152,
    /// <summary>Invalid value for registry</summary>
    REGDB_E_INVALIDVALUE = 0x80040153,
    /// <summary>Class not registered</summary>
    REGDB_E_CLASSNOTREG = 0x80040154,
    /// <summary>Interface not registered</summary>
    REGDB_E_IIDNOTREG = 0x80040155,
    /// <summary>Threading model entry is not valid</summary>
    REGDB_E_BADTHREADINGMODEL = 0x80040156,
    /// <summary>CATID does not exist</summary>
    CAT_E_CATIDNOEXIST = 0x80040160,
    /// <summary>Description not found</summary>
    CAT_E_NODESCRIPTION = 0x80040161,
    /// <summary>No package in the software installation data in the Active Directory meets this criteria.</summary>
    CS_E_PACKAGE_NOTFOUND = 0x80040164,
    /// <summary>Deleting this will break the referential integrity of the software installation data in the Active Directory.</summary>
    CS_E_NOT_DELETABLE = 0x80040165,
    /// <summary>The CLSID was not found in the software installation data in the Active Directory.</summary>
    CS_E_CLASS_NOTFOUND = 0x80040166,
    /// <summary>The software installation data in the Active Directory is corrupt.</summary>
    CS_E_INVALID_VERSION = 0x80040167,
    /// <summary>There is no software installation data in the Active Directory.</summary>
    CS_E_NO_CLASSSTORE = 0x80040168,
    /// <summary>There is no software installation data object in the Active Directory.</summary>
    CS_E_OBJECT_NOTFOUND = 0x80040169,
    /// <summary>The software installation data object in the Active Directory already exists.</summary>
    CS_E_OBJECT_ALREADY_EXISTS = 0x8004016A,
    /// <summary>The path to the software installation data in the Active Directory is not correct.</summary>
    CS_E_INVALID_PATH = 0x8004016B,
    /// <summary>A network error interrupted the operation.</summary>
    CS_E_NETWORK_ERROR = 0x8004016C,
    /// <summary>The size of this object exceeds the maximum size set by the Administrator.</summary>
    CS_E_ADMIN_LIMIT_EXCEEDED = 0x8004016D,
    /// <summary>The schema for the software installation data in the Active Directory does not match the required schema.</summary>
    CS_E_SCHEMA_MISMATCH = 0x8004016E,
    /// <summary>An error occurred in the software installation data in the Active Directory.</summary>
    CS_E_INTERNAL_ERROR = 0x8004016F,
    /// <summary>Cache not updated</summary>
    CACHE_E_NOCACHE_UPDATED = 0x80040170,
    /// <summary>No verbs for OLE object</summary>
    OLEOBJ_E_NOVERBS = 0x80040180,
    /// <summary>Invalid verb for OLE object</summary>
    OLEOBJ_E_INVALIDVERB = 0x80040181,
    /// <summary>Undo is not available</summary>
    INPLACE_E_NOTUNDOABLE = 0x800401A0,
    /// <summary>Space for tools is not available</summary>
    INPLACE_E_NOTOOLSPACE = 0x800401A1,
    /// <summary>OLESTREAM Get method failed</summary>
    CONVERT10_E_OLESTREAM_GET = 0x800401C0,
    /// <summary>OLESTREAM Put method failed</summary>
    CONVERT10_E_OLESTREAM_PUT = 0x800401C1,
    /// <summary>Contents of the OLESTREAM not in correct format</summary>
    CONVERT10_E_OLESTREAM_FMT = 0x800401C2,
    /// <summary>There was an error in a Windows GDI call while converting the bitmap to a DIB</summary>
    CONVERT10_E_OLESTREAM_BITMAP_TO_DIB = 0x800401C3,
    /// <summary>Contents of the IStorage not in correct format</summary>
    CONVERT10_E_STG_FMT = 0x800401C4,
    /// <summary>Contents of IStorage is missing one of the standard streams</summary>
    CONVERT10_E_STG_NO_STD_STREAM = 0x800401C5,
    /// <summary>There was an error in a Windows GDI call while converting the DIB to a bitmap.</summary>
    CONVERT10_E_STG_DIB_TO_BITMAP = 0x800401C6,
    /// <summary>OpenClipboard Failed</summary>
    CLIPBRD_E_CANT_OPEN = 0x800401D0,
    /// <summary>EmptyClipboard Failed</summary>
    CLIPBRD_E_CANT_EMPTY = 0x800401D1,
    /// <summary>SetClipboard Failed</summary>
    CLIPBRD_E_CANT_SET = 0x800401D2,
    /// <summary>Data on clipboard is invalid</summary>
    CLIPBRD_E_BAD_DATA = 0x800401D3,
    /// <summary>CloseClipboard Failed</summary>
    CLIPBRD_E_CANT_CLOSE = 0x800401D4,
    /// <summary>Moniker needs to be connected manually</summary>
    MK_E_CONNECTMANUALLY = 0x800401E0,
    /// <summary>Operation exceeded deadline</summary>
    MK_E_EXCEEDEDDEADLINE = 0x800401E1,
    /// <summary>Moniker needs to be generic</summary>
    MK_E_NEEDGENERIC = 0x800401E2,
    /// <summary>Operation unavailable</summary>
    MK_E_UNAVAILABLE = 0x800401E3,
    /// <summary>Invalid syntax</summary>
    MK_E_SYNTAX = 0x800401E4,
    /// <summary>No object for moniker</summary>
    MK_E_NOOBJECT = 0x800401E5,
    /// <summary>Bad extension for file</summary>
    MK_E_INVALIDEXTENSION = 0x800401E6,
    /// <summary>Intermediate operation failed</summary>
    MK_E_INTERMEDIATEINTERFACENOTSUPPORTED = 0x800401E7,
    /// <summary>Moniker is not bindable</summary>
    MK_E_NOTBINDABLE = 0x800401E8,
    /// <summary>Moniker is not bound</summary>
    MK_E_NOTBOUND = 0x800401E9,
    /// <summary>Moniker cannot open file</summary>
    MK_E_CANTOPENFILE = 0x800401EA,
    /// <summary>User input required for operation to succeed</summary>
    MK_E_MUSTBOTHERUSER = 0x800401EB,
    /// <summary>Moniker class has no inverse</summary>
    MK_E_NOINVERSE = 0x800401EC,
    /// <summary>Moniker does not refer to storage</summary>
    MK_E_NOSTORAGE = 0x800401ED,
    /// <summary>No common prefix</summary>
    MK_E_NOPREFIX = 0x800401EE,
    /// <summary>Moniker could not be enumerated</summary>
    MK_E_ENUMERATION_FAILED = 0x800401EF,
    /// <summary>CoInitialize has not been called.</summary>
    CO_E_NOTINITIALIZED = 0x800401F0,
    /// <summary>CoInitialize has already been called.</summary>
    CO_E_ALREADYINITIALIZED = 0x800401F1,
    /// <summary>Class of object cannot be determined</summary>
    CO_E_CANTDETERMINECLASS = 0x800401F2,
    /// <summary>Invalid class string</summary>
    CO_E_CLASSSTRING = 0x800401F3,
    /// <summary>Invalid interface string</summary>
    CO_E_IIDSTRING = 0x800401F4,
    /// <summary>Application not found</summary>
    CO_E_APPNOTFOUND = 0x800401F5,
    /// <summary>Application cannot be run more than once</summary>
    CO_E_APPSINGLEUSE = 0x800401F6,
    /// <summary>Some error in application program</summary>
    CO_E_ERRORINAPP = 0x800401F7,
    /// <summary>DLL for class not found</summary>
    CO_E_DLLNOTFOUND = 0x800401F8,
    /// <summary>Error in the DLL</summary>
    CO_E_ERRORINDLL = 0x800401F9,
    /// <summary>Wrong OS or OS version for application</summary>
    CO_E_WRONGOSFORAPP = 0x800401FA,
    /// <summary>Object is not registered</summary>
    CO_E_OBJNOTREG = 0x800401FB,
    /// <summary>Object is already registered</summary>
    CO_E_OBJISREG = 0x800401FC,
    /// <summary>Object is not connected to server</summary>
    CO_E_OBJNOTCONNECTED = 0x800401FD,
    /// <summary>Application was launched but it didn&#39;t register a class factory</summary>
    CO_E_APPDIDNTREG = 0x800401FE,
    /// <summary>Object has been released</summary>
    CO_E_RELEASED = 0x800401FF,
    /// <summary>An event was able to invoke some but not all of the subscribers</summary>
    EVENT_S_SOME_SUBSCRIBERS_FAILED = 0x00040200,
    /// <summary>An event was unable to invoke any of the subscribers</summary>
    EVENT_E_ALL_SUBSCRIBERS_FAILED = 0x80040201,
    /// <summary>An event was delivered but there were no subscribers</summary>
    EVENT_S_NOSUBSCRIBERS = 0x00040202,
    /// <summary>A syntax error occurred trying to evaluate a query string</summary>
    EVENT_E_QUERYSYNTAX = 0x80040203,
    /// <summary>An invalid field name was used in a query string</summary>
    EVENT_E_QUERYFIELD = 0x80040204,
    /// <summary>An unexpected exception was raised</summary>
    EVENT_E_INTERNALEXCEPTION = 0x80040205,
    /// <summary>An unexpected internal error was detected</summary>
    EVENT_E_INTERNALERROR = 0x80040206,
    /// <summary>The owner SID on a per-user subscription doesn&#39;t exist</summary>
    EVENT_E_INVALID_PER_USER_SID = 0x80040207,
    /// <summary>A user-supplied component or subscriber raised an exception</summary>
    EVENT_E_USER_EXCEPTION = 0x80040208,
    /// <summary>An interface has too many methods to fire events from</summary>
    EVENT_E_TOO_MANY_METHODS = 0x80040209,
    /// <summary>A subscription cannot be stored unless its event class already exists</summary>
    EVENT_E_MISSING_EVENTCLASS = 0x8004020A,
    /// <summary>Not all the objects requested could be removed</summary>
    EVENT_E_NOT_ALL_REMOVED = 0x8004020B,
    /// <summary>COM+ is required for this operation, but is not installed</summary>
    EVENT_E_COMPLUS_NOT_INSTALLED = 0x8004020C,
    /// <summary>Cannot modify or delete an object that was not added using the COM+ Admin SDK</summary>
    EVENT_E_CANT_MODIFY_OR_DELETE_UNCONFIGURED_OBJECT = 0x8004020D,
    /// <summary>Cannot modify or delete an object that was added using the COM+ Admin SDK</summary>
    EVENT_E_CANT_MODIFY_OR_DELETE_CONFIGURED_OBJECT = 0x8004020E,
    /// <summary>The event class for this subscription is in an invalid partition</summary>
    EVENT_E_INVALID_EVENT_CLASS_PARTITION = 0x8004020F,
    /// <summary>The owner of the PerUser subscription is not logged on to the system specified</summary>
    EVENT_E_PER_USER_SID_NOT_LOGGED_ON = 0x80040210,
    /// <summary>Another single phase resource manager has already been enlisted in this transaction.</summary>
    XACT_E_ALREADYOTHERSINGLEPHASE = 0x8004D000,
    /// <summary>A retaining commit or abort is not supported</summary>
    XACT_E_CANTRETAIN = 0x8004D001,
    /// <summary>The transaction failed to commit for an unknown reason. The transaction was aborted.</summary>
    XACT_E_COMMITFAILED = 0x8004D002,
    /// <summary>Cannot call commit on this transaction object because the calling application did not initiate the transaction.</summary>
    XACT_E_COMMITPREVENTED = 0x8004D003,
    /// <summary>Instead of committing, the resource heuristically aborted.</summary>
    XACT_E_HEURISTICABORT = 0x8004D004,
    /// <summary>Instead of aborting, the resource heuristically committed.</summary>
    XACT_E_HEURISTICCOMMIT = 0x8004D005,
    /// <summary>Some of the states of the resource were committed while others were aborted, likely because of heuristic decisions.</summary>
    XACT_E_HEURISTICDAMAGE = 0x8004D006,
    /// <summary>Some of the states of the resource may have been committed while others may have been aborted, likely because of heuristic decisions.</summary>
    XACT_E_HEURISTICDANGER = 0x8004D007,
    /// <summary>The requested isolation level is not valid or supported.</summary>
    XACT_E_ISOLATIONLEVEL = 0x8004D008,
    /// <summary>The transaction manager doesn&#39;t support an asynchronous operation for this method.</summary>
    XACT_E_NOASYNC = 0x8004D009,
    /// <summary>Unable to enlist in the transaction.</summary>
    XACT_E_NOENLIST = 0x8004D00A,
    /// <summary>The requested semantics of retention of isolation across retaining commit and abort boundaries cannot be supported by this transaction implementation, or isoFlags was not equal to zero.</summary>
    XACT_E_NOISORETAIN = 0x8004D00B,
    /// <summary>There is no resource presently associated with this enlistment</summary>
    XACT_E_NORESOURCE = 0x8004D00C,
    /// <summary>The transaction failed to commit due to the failure of optimistic concurrency control in at least one of the resource managers.</summary>
    XACT_E_NOTCURRENT = 0x8004D00D,
    /// <summary>The transaction has already been implicitly or explicitly committed or aborted</summary>
    XACT_E_NOTRANSACTION = 0x8004D00E,
    /// <summary>An invalid combination of flags was specified</summary>
    XACT_E_NOTSUPPORTED = 0x8004D00F,
    /// <summary>The resource manager id is not associated with this transaction or the transaction manager.</summary>
    XACT_E_UNKNOWNRMGRID = 0x8004D010,
    /// <summary>This method was called in the wrong state</summary>
    XACT_E_WRONGSTATE = 0x8004D011,
    /// <summary>The indicated unit of work does not match the unit of work expected by the resource manager.</summary>
    XACT_E_WRONGUOW = 0x8004D012,
    /// <summary>An enlistment in a transaction already exists.</summary>
    XACT_E_XTIONEXISTS = 0x8004D013,
    /// <summary>An import object for the transaction could not be found.</summary>
    XACT_E_NOIMPORTOBJECT = 0x8004D014,
    /// <summary>The transaction cookie is invalid.</summary>
    XACT_E_INVALIDCOOKIE = 0x8004D015,
    /// <summary>The transaction status is in doubt. A communication failure occurred, or a transaction manager or resource manager has failed</summary>
    XACT_E_INDOUBT = 0x8004D016,
    /// <summary>A time-out was specified, but time-outs are not supported.</summary>
    XACT_E_NOTIMEOUT = 0x8004D017,
    /// <summary>The requested operation is already in progress for the transaction.</summary>
    XACT_E_ALREADYINPROGRESS = 0x8004D018,
    /// <summary>The transaction has already been aborted.</summary>
    XACT_E_ABORTED = 0x8004D019,
    /// <summary>The Transaction Manager returned a log full error.</summary>
    XACT_E_LOGFULL = 0x8004D01A,
    /// <summary>The Transaction Manager is not available.</summary>
    XACT_E_TMNOTAVAILABLE = 0x8004D01B,
    /// <summary>A connection with the transaction manager was lost.</summary>
    XACT_E_CONNECTION_DOWN = 0x8004D01C,
    /// <summary>A request to establish a connection with the transaction manager was denied.</summary>
    XACT_E_CONNECTION_DENIED = 0x8004D01D,
    /// <summary>Resource manager reenlistment to determine transaction status timed out.</summary>
    XACT_E_REENLISTTIMEOUT = 0x8004D01E,
    /// <summary>This transaction manager failed to establish a connection with another TIP transaction manager.</summary>
    XACT_E_TIP_CONNECT_FAILED = 0x8004D01F,
    /// <summary>This transaction manager encountered a protocol error with another TIP transaction manager.</summary>
    XACT_E_TIP_PROTOCOL_ERROR = 0x8004D020,
    /// <summary>This transaction manager could not propagate a transaction from another TIP transaction manager.</summary>
    XACT_E_TIP_PULL_FAILED = 0x8004D021,
    /// <summary>The Transaction Manager on the destination machine is not available.</summary>
    XACT_E_DEST_TMNOTAVAILABLE = 0x8004D022,
    /// <summary>The Transaction Manager has disabled its support for TIP.</summary>
    XACT_E_TIP_DISABLED = 0x8004D023,
    /// <summary>The transaction manager has disabled its support for remote/network transactions.</summary>
    XACT_E_NETWORK_TX_DISABLED = 0x8004D024,
    /// <summary>The partner transaction manager has disabled its support for remote/network transactions.</summary>
    XACT_E_PARTNER_NETWORK_TX_DISABLED = 0x8004D025,
    /// <summary>The transaction manager has disabled its support for XA transactions.</summary>
    XACT_E_XA_TX_DISABLED = 0x8004D026,
    /// <summary>MSDTC was unable to read its configuration information.</summary>
    XACT_E_UNABLE_TO_READ_DTC_CONFIG = 0x8004D027,
    /// <summary>MSDTC was unable to load the dtc proxy dll.</summary>
    XACT_E_UNABLE_TO_LOAD_DTC_PROXY = 0x8004D028,
    /// <summary>The local transaction has aborted.</summary>
    XACT_E_ABORTING = 0x8004D029,
    /// <summary>The MSDTC transaction manager was unable to push the transaction to the destination transaction manager due to communication problems. Possible causes are: a firewall is present and it doesn&#39;t have an exception for the MSDTC process, the two machines cannot find each other by their NetBIOS names, or the support for network transactions is not enabled for one of the two transaction managers.</summary>
    XACT_E_PUSH_COMM_FAILURE = 0x8004D02A,
    /// <summary>The MSDTC transaction manager was unable to pull the transaction from the source transaction manager due to communication problems. Possible causes are: a firewall is present and it doesn&#39;t have an exception for the MSDTC process, the two machines cannot find each other by their NetBIOS names, or the support for network transactions is not enabled for one of the two transaction managers.</summary>
    XACT_E_PULL_COMM_FAILURE = 0x8004D02B,
    /// <summary>The MSDTC transaction manager has disabled its support for SNA LU 6.2 transactions.</summary>
    XACT_E_LU_TX_DISABLED = 0x8004D02C,
    /// <summary> XACT_E_CLERKNOTFOUND</summary>
    XACT_E_CLERKNOTFOUND = 0x8004D080,
    /// <summary> XACT_E_CLERKEXISTS</summary>
    XACT_E_CLERKEXISTS = 0x8004D081,
    /// <summary> XACT_E_RECOVERYINPROGRESS</summary>
    XACT_E_RECOVERYINPROGRESS = 0x8004D082,
    /// <summary> XACT_E_TRANSACTIONCLOSED</summary>
    XACT_E_TRANSACTIONCLOSED = 0x8004D083,
    /// <summary> XACT_E_INVALIDLSN</summary>
    XACT_E_INVALIDLSN = 0x8004D084,
    /// <summary> XACT_E_REPLAYREQUEST</summary>
    XACT_E_REPLAYREQUEST = 0x8004D085,
    /// <summary>The request to connect to the specified transaction coordinator was denied. SymbolicName=XACT_E_TOOMANY_ENLISTMENTS MessageId: 0x8004D101L (No symbolic name defined) The maximum number of enlistments for the specified transaction has been reached. SymbolicName=XACT_E_DUPLICATE_GUID MessageId: 0x8004D102L (No symbolic name defined) A resource manager with the same identifier is already registered with the specified transaction coordinator. SymbolicName=XACT_E_NOTSINGLEPHASE MessageId: 0x8004D103L (No symbolic name defined) The prepare request given was not eligible for single phase optimizations. SymbolicName=XACT_E_RECOVERYALREADYDONE MessageId: 0x8004D104L (No symbolic name defined) RecoveryComplete has already been called for the given resource manager. SymbolicName=XACT_E_PROTOCOL MessageId: 0x8004D105L (No symbolic name defined) The interface call made was incorrect for the current state of the protocol. SymbolicName=XACT_E_RM_FAILURE MessageId: 0x8004D106L (No symbolic name defined) xa_open call failed for the XA resource. SymbolicName=XACT_E_RECOVERY_FAILED MessageId: 0x8004D107L (No symbolic name defined) xa_recover call failed for the XA resource. SymbolicName=XACT_E_LU_NOT_FOUND MessageId: 0x8004D108L (No symbolic name defined) The Logical Unit of Work specified cannot be found. SymbolicName=XACT_E_DUPLICATE_LU MessageId: 0x8004D109L (No symbolic name defined) The specified Logical Unit of Work already exists. SymbolicName=XACT_E_LU_NOT_CONNECTED MessageId: 0x8004D10AL (No symbolic name defined) Subordinate creation failed. The specified Logical Unit of Work was not connected. SymbolicName=XACT_E_DUPLICATE_TRANSID MessageId: 0x8004D10BL (No symbolic name defined) A transaction with the given identifier already exists. SymbolicName=XACT_E_LU_BUSY MessageId: 0x8004D10CL (No symbolic name defined) The resource is in use. SymbolicName=XACT_E_LU_NO_RECOVERY_PROCESS MessageId: 0x8004D10DL (No symbolic name defined) The LU Recovery process is down. SymbolicName=XACT_E_LU_DOWN MessageId: 0x8004D10EL (No symbolic name defined) The remote session was lost. SymbolicName=XACT_E_LU_RECOVERING MessageId: 0x8004D10FL (No symbolic name defined) The resource is currently recovering. SymbolicName=XACT_E_LU_RECOVERY_MISMATCH MessageId: 0x8004D110L (No symbolic name defined) There was a mismatch in driving recovery. SymbolicName=XACT_E_RM_UNAVAILABLE MessageId: 0x8004D111L (No symbolic name defined) An error occurred with the XA resource. End XACT_DTC_CONSTANTS enumerated values defined in txdtc.h OleTx Success codes. MessageId: XACT_S_ASYNC An asynchronous operation was specified. The operation has begun, but its outcome is not known yet.</summary>
    XACT_S_ASYNC = 0x0004D000,
    /// <summary> XACT_S_DEFECT</summary>
    XACT_S_DEFECT = 0x0004D001,
    /// <summary>The method call succeeded because the transaction was read-only.</summary>
    XACT_S_READONLY = 0x0004D002,
    /// <summary>The transaction was successfully aborted. However, this is a coordinated transaction, and some number of enlisted resources were aborted outright because they could not support abort-retaining semantics</summary>
    XACT_S_SOMENORETAIN = 0x0004D003,
    /// <summary>No changes were made during this call, but the sink wants another chance to look if any other sinks make further changes.</summary>
    XACT_S_OKINFORM = 0x0004D004,
    /// <summary>The sink is content and wishes the transaction to proceed. Changes were made to one or more resources during this call.</summary>
    XACT_S_MADECHANGESCONTENT = 0x0004D005,
    /// <summary>The sink is for the moment and wishes the transaction to proceed, but if other changes are made following this return by other event sinks then this sink wants another chance to look</summary>
    XACT_S_MADECHANGESINFORM = 0x0004D006,
    /// <summary>The transaction was successfully aborted. However, the abort was non-retaining.</summary>
    XACT_S_ALLNORETAIN = 0x0004D007,
    /// <summary>An abort operation was already in progress.</summary>
    XACT_S_ABORTING = 0x0004D008,
    /// <summary>The resource manager has performed a single-phase commit of the transaction.</summary>
    XACT_S_SINGLEPHASE = 0x0004D009,
    /// <summary>The local transaction has not aborted.</summary>
    XACT_S_LOCALLY_OK = 0x0004D00A,
    /// <summary>The resource manager has requested to be the coordinator (last resource manager) for the transaction.</summary>
    XACT_S_LASTRESOURCEMANAGER = 0x0004D010,
    /// <summary>The root transaction wanted to commit, but transaction aborted</summary>
    CONTEXT_E_ABORTED = 0x8004E002,
    /// <summary>You made a method call on a COM+ component that has a transaction that has already aborted or in the process of aborting.</summary>
    CONTEXT_E_ABORTING = 0x8004E003,
    /// <summary>There is no MTS object context</summary>
    CONTEXT_E_NOCONTEXT = 0x8004E004,
    /// <summary>The component is configured to use synchronization and this method call would cause a deadlock to occur.</summary>
    CONTEXT_E_WOULD_DEADLOCK = 0x8004E005,
    /// <summary>The component is configured to use synchronization and a thread has timed out waiting to enter the context.</summary>
    CONTEXT_E_SYNCH_TIMEOUT = 0x8004E006,
    /// <summary>You made a method call on a COM+ component that has a transaction that has already committed or aborted.</summary>
    CONTEXT_E_OLDREF = 0x8004E007,
    /// <summary>The specified role was not configured for the application</summary>
    CONTEXT_E_ROLENOTFOUND = 0x8004E00C,
    /// <summary>COM+ was unable to talk to the Microsoft Distributed Transaction Coordinator</summary>
    CONTEXT_E_TMNOTAVAILABLE = 0x8004E00F,
    /// <summary>An unexpected error occurred during COM+ Activation.</summary>
    CO_E_ACTIVATIONFAILED = 0x8004E021,
    /// <summary>COM+ Activation failed. Check the event log for more information</summary>
    CO_E_ACTIVATIONFAILED_EVENTLOGGED = 0x8004E022,
    /// <summary>COM+ Activation failed due to a catalog or configuration error.</summary>
    CO_E_ACTIVATIONFAILED_CATALOGERROR = 0x8004E023,
    /// <summary>COM+ activation failed because the activation could not be completed in the specified amount of time.</summary>
    CO_E_ACTIVATIONFAILED_TIMEOUT = 0x8004E024,
    /// <summary>COM+ Activation failed because an initialization function failed. Check the event log for more information.</summary>
    CO_E_INITIALIZATIONFAILED = 0x8004E025,
    /// <summary>The requested operation requires that JIT be in the current context and it is not</summary>
    CONTEXT_E_NOJIT = 0x8004E026,
    /// <summary>The requested operation requires that the current context have a Transaction, and it does not</summary>
    CONTEXT_E_NOTRANSACTION = 0x8004E027,
    /// <summary>The components threading model has changed after install into a COM+ Application. Please re-install component.</summary>
    CO_E_THREADINGMODEL_CHANGED = 0x8004E028,
    /// <summary>IIS intrinsics not available. Start your work with IIS.</summary>
    CO_E_NOIISINTRINSICS = 0x8004E029,
    /// <summary>An attempt to write a cookie failed.</summary>
    CO_E_NOCOOKIES = 0x8004E02A,
    /// <summary>An attempt to use a database generated a database specific error.</summary>
    CO_E_DBERROR = 0x8004E02B,
    /// <summary>The COM+ component you created must use object pooling to work.</summary>
    CO_E_NOTPOOLED = 0x8004E02C,
    /// <summary>The COM+ component you created must use object construction to work correctly.</summary>
    CO_E_NOTCONSTRUCTED = 0x8004E02D,
    /// <summary>The COM+ component requires synchronization, and it is not configured for it.</summary>
    CO_E_NOSYNCHRONIZATION = 0x8004E02E,
    /// <summary>The TxIsolation Level property for the COM+ component being created is stronger than the TxIsolationLevel for the &quot;root&quot; component for the transaction. The creation failed.</summary>
    CO_E_ISOLEVELMISMATCH = 0x8004E02F,
    /// <summary>The component attempted to make a cross-context call between invocations of EnterTransactionScopeand ExitTransactionScope. This is not allowed. Cross-context calls cannot be made while inside of a transaction scope.</summary>
    CO_E_CALL_OUT_OF_TX_SCOPE_NOT_ALLOWED = 0x8004E030,
    /// <summary>The component made a call to EnterTransactionScope, but did not make a corresponding call to ExitTransactionScope before returning.</summary>
    CO_E_EXIT_TRANSACTION_SCOPE_NOT_CALLED = 0x8004E031,
    /// <summary>Use the registry database to provide the requested information</summary>
    OLE_S_USEREG = 0x00040000,
    /// <summary>Success, but static</summary>
    OLE_S_STATIC = 0x00040001,
    /// <summary>Macintosh clipboard format</summary>
    OLE_S_MAC_CLIPFORMAT = 0x00040002,
    /// <summary>Successful drop took place</summary>
    DRAGDROP_S_DROP = 0x00040100,
    /// <summary>Drag-drop operation canceled</summary>
    DRAGDROP_S_CANCEL = 0x00040101,
    /// <summary>Use the default cursor</summary>
    DRAGDROP_S_USEDEFAULTCURSORS = 0x00040102,
    /// <summary>Data has same FORMATETC</summary>
    DATA_S_SAMEFORMATETC = 0x00040130,
    /// <summary>View is already frozen</summary>
    VIEW_S_ALREADY_FROZEN = 0x00040140,
    /// <summary>FORMATETC not supported</summary>
    CACHE_S_FORMATETC_NOTSUPPORTED = 0x00040170,
    /// <summary>Same cache</summary>
    CACHE_S_SAMECACHE = 0x00040171,
    /// <summary>Some cache(s) not updated</summary>
    CACHE_S_SOMECACHES_NOTUPDATED = 0x00040172,
    /// <summary>Invalid verb for OLE object</summary>
    OLEOBJ_S_INVALIDVERB = 0x00040180,
    /// <summary>Verb number is valid but verb cannot be done now</summary>
    OLEOBJ_S_CANNOT_DOVERB_NOW = 0x00040181,
    /// <summary>Invalid window handle passed</summary>
    OLEOBJ_S_INVALIDHWND = 0x00040182,
    /// <summary>Message is too long; some of it had to be truncated before displaying</summary>
    INPLACE_S_TRUNCATED = 0x000401A0,
    /// <summary>Unable to convert OLESTREAM to IStorage</summary>
    CONVERT10_S_NO_PRESENTATION = 0x000401C0,
    /// <summary>Moniker reduced to itself</summary>
    MK_S_REDUCED_TO_SELF = 0x000401E2,
    /// <summary>Common prefix is this moniker</summary>
    MK_S_ME = 0x000401E4,
    /// <summary>Common prefix is input moniker</summary>
    MK_S_HIM = 0x000401E5,
    /// <summary>Common prefix is both monikers</summary>
    MK_S_US = 0x000401E6,
    /// <summary>Moniker is already registered in running object table</summary>
    MK_S_MONIKERALREADYREGISTERED = 0x000401E7,
    /// <summary>The task is ready to run at its next scheduled time.</summary>
    SCHED_S_TASK_READY = 0x00041300,
    /// <summary>The task is currently running.</summary>
    SCHED_S_TASK_RUNNING = 0x00041301,
    /// <summary>The task will not run at the scheduled times because it has been disabled.</summary>
    SCHED_S_TASK_DISABLED = 0x00041302,
    /// <summary>The task has not yet run.</summary>
    SCHED_S_TASK_HAS_NOT_RUN = 0x00041303,
    /// <summary>There are no more runs scheduled for this task.</summary>
    SCHED_S_TASK_NO_MORE_RUNS = 0x00041304,
    /// <summary>One or more of the properties that are needed to run this task on a schedule have not been set.</summary>
    SCHED_S_TASK_NOT_SCHEDULED = 0x00041305,
    /// <summary>The last run of the task was terminated by the user.</summary>
    SCHED_S_TASK_TERMINATED = 0x00041306,
    /// <summary>Either the task has no triggers or the existing triggers are disabled or not set.</summary>
    SCHED_S_TASK_NO_VALID_TRIGGERS = 0x00041307,
    /// <summary>Event triggers don&#39;t have set run times.</summary>
    SCHED_S_EVENT_TRIGGER = 0x00041308,
    /// <summary>Trigger not found.</summary>
    SCHED_E_TRIGGER_NOT_FOUND = 0x80041309,
    /// <summary>One or more of the properties that are needed to run this task have not been set.</summary>
    SCHED_E_TASK_NOT_READY = 0x8004130A,
    /// <summary>There is no running instance of the task.</summary>
    SCHED_E_TASK_NOT_RUNNING = 0x8004130B,
    /// <summary>The Task Scheduler Service is not installed on this computer.</summary>
    SCHED_E_SERVICE_NOT_INSTALLED = 0x8004130C,
    /// <summary>The task object could not be opened.</summary>
    SCHED_E_CANNOT_OPEN_TASK = 0x8004130D,
    /// <summary>The object is either an invalid task object or is not a task object.</summary>
    SCHED_E_INVALID_TASK = 0x8004130E,
    /// <summary>No account information could be found in the Task Scheduler security database for the task indicated.</summary>
    SCHED_E_ACCOUNT_INFORMATION_NOT_SET = 0x8004130F,
    /// <summary>Unable to establish existence of the account specified.</summary>
    SCHED_E_ACCOUNT_NAME_NOT_FOUND = 0x80041310,
    /// <summary>Corruption was detected in the Task Scheduler security database; the database has been reset.</summary>
    SCHED_E_ACCOUNT_DBASE_CORRUPT = 0x80041311,
    /// <summary>Task Scheduler security services are available only on Windows NT.</summary>
    SCHED_E_NO_SECURITY_SERVICES = 0x80041312,
    /// <summary>The task object version is either unsupported or invalid.</summary>
    SCHED_E_UNKNOWN_OBJECT_VERSION = 0x80041313,
    /// <summary>The task has been configured with an unsupported combination of account settings and run time options.</summary>
    SCHED_E_UNSUPPORTED_ACCOUNT_OPTION = 0x80041314,
    /// <summary>The Task Scheduler Service is not running.</summary>
    SCHED_E_SERVICE_NOT_RUNNING = 0x80041315,
    /// <summary>The task XML contains an unexpected node.</summary>
    SCHED_E_UNEXPECTEDNODE = 0x80041316,
    /// <summary>The task XML contains an element or attribute from an unexpected namespace.</summary>
    SCHED_E_NAMESPACE = 0x80041317,
    /// <summary>The task XML contains a value which is incorrectly formatted or out of range.</summary>
    SCHED_E_INVALIDVALUE = 0x80041318,
    /// <summary>The task XML is missing a required element or attribute.</summary>
    SCHED_E_MISSINGNODE = 0x80041319,
    /// <summary>The task XML is malformed.</summary>
    SCHED_E_MALFORMEDXML = 0x8004131A,
    /// <summary>The task is registered, but not all specified triggers will start the task.</summary>
    SCHED_S_SOME_TRIGGERS_FAILED = 0x0004131B,
    /// <summary>The task is registered, but may fail to start. Batch logon privilege needs to be enabled for the task principal.</summary>
    SCHED_S_BATCH_LOGON_PROBLEM = 0x0004131C,
    /// <summary>The task XML contains too many nodes of the same type.</summary>
    SCHED_E_TOO_MANY_NODES = 0x8004131D,
    /// <summary>The task cannot be started after the trigger&#39;s end boundary.</summary>
    SCHED_E_PAST_END_BOUNDARY = 0x8004131E,
    /// <summary>An instance of this task is already running.</summary>
    SCHED_E_ALREADY_RUNNING = 0x8004131F,
    /// <summary>The task will not run because the user is not logged on.</summary>
    SCHED_E_USER_NOT_LOGGED_ON = 0x80041320,
    /// <summary>The task image is corrupt or has been tampered with.</summary>
    SCHED_E_INVALID_TASK_HASH = 0x80041321,
    /// <summary>The Task Scheduler service is not available.</summary>
    SCHED_E_SERVICE_NOT_AVAILABLE = 0x80041322,
    /// <summary>The Task Scheduler service is too busy to handle your request. Please try again later.</summary>
    SCHED_E_SERVICE_TOO_BUSY = 0x80041323,
    /// <summary>The Task Scheduler service attempted to run the task, but the task did not run due to one of the constraints in the task definition.</summary>
    SCHED_E_TASK_ATTEMPTED = 0x80041324,
    /// <summary>The Task Scheduler service has asked the task to run.</summary>
    SCHED_S_TASK_QUEUED = 0x00041325,
    /// <summary>The task is disabled.</summary>
    SCHED_E_TASK_DISABLED = 0x80041326,
    /// <summary>The task has properties that are not compatible with previous versions of Windows.</summary>
    SCHED_E_TASK_NOT_V1_COMPAT = 0x80041327,
    /// <summary>The task settings do not allow the task to start on demand.</summary>
    SCHED_E_START_ON_DEMAND = 0x80041328,
    /// <summary>Attempt to create a class object failed</summary>
    CO_E_CLASS_CREATE_FAILED = 0x80080001,
    /// <summary>OLE service could not bind object</summary>
    CO_E_SCM_ERROR = 0x80080002,
    /// <summary>RPC communication failed with OLE service</summary>
    CO_E_SCM_RPC_FAILURE = 0x80080003,
    /// <summary>Bad path to object</summary>
    CO_E_BAD_PATH = 0x80080004,
    /// <summary>Server execution failed</summary>
    CO_E_SERVER_EXEC_FAILURE = 0x80080005,
    /// <summary>OLE service could not communicate with the object server</summary>
    CO_E_OBJSRV_RPC_FAILURE = 0x80080006,
    /// <summary>Moniker path could not be normalized</summary>
    MK_E_NO_NORMALIZED = 0x80080007,
    /// <summary>Object server is stopping when OLE service contacts it</summary>
    CO_E_SERVER_STOPPING = 0x80080008,
    /// <summary>An invalid root block pointer was specified</summary>
    MEM_E_INVALID_ROOT = 0x80080009,
    /// <summary>An allocation chain contained an invalid link pointer</summary>
    MEM_E_INVALID_LINK = 0x80080010,
    /// <summary>The requested allocation size was too large</summary>
    MEM_E_INVALID_SIZE = 0x80080011,
    /// <summary>Not all the requested interfaces were available</summary>
    CO_S_NOTALLINTERFACES = 0x00080012,
    /// <summary>The specified machine name was not found in the cache.</summary>
    CO_S_MACHINENAMENOTFOUND = 0x00080013,
    /// <summary>The activation requires a display name to be present under the CLSID key.</summary>
    CO_E_MISSING_DISPLAYNAME = 0x80080015,
    /// <summary>The activation requires that the RunAs value for the application is Activate As Activator.</summary>
    CO_E_RUNAS_VALUE_MUST_BE_AAA = 0x80080016,
    /// <summary>The class is not configured to support Elevated activation.</summary>
    CO_E_ELEVATION_DISABLED = 0x80080017,
    /// <summary>Unknown interface.</summary>
    DISP_E_UNKNOWNINTERFACE = 0x80020001,
    /// <summary>Member not found.</summary>
    DISP_E_MEMBERNOTFOUND = 0x80020003,
    /// <summary>Parameter not found.</summary>
    DISP_E_PARAMNOTFOUND = 0x80020004,
    /// <summary>Type mismatch.</summary>
    DISP_E_TYPEMISMATCH = 0x80020005,
    /// <summary>Unknown name.</summary>
    DISP_E_UNKNOWNNAME = 0x80020006,
    /// <summary>No named arguments.</summary>
    DISP_E_NONAMEDARGS = 0x80020007,
    /// <summary>Bad variable type.</summary>
    DISP_E_BADVARTYPE = 0x80020008,
    /// <summary>Exception occurred.</summary>
    DISP_E_EXCEPTION = 0x80020009,
    /// <summary>Out of present range.</summary>
    DISP_E_OVERFLOW = 0x8002000A,
    /// <summary>Invalid index.</summary>
    DISP_E_BADINDEX = 0x8002000B,
    /// <summary>Unknown language.</summary>
    DISP_E_UNKNOWNLCID = 0x8002000C,
    /// <summary>Memory is locked.</summary>
    DISP_E_ARRAYISLOCKED = 0x8002000D,
    /// <summary>Invalid number of parameters.</summary>
    DISP_E_BADPARAMCOUNT = 0x8002000E,
    /// <summary>Parameter not optional.</summary>
    DISP_E_PARAMNOTOPTIONAL = 0x8002000F,
    /// <summary>Invalid callee.</summary>
    DISP_E_BADCALLEE = 0x80020010,
    /// <summary>Does not support a collection.</summary>
    DISP_E_NOTACOLLECTION = 0x80020011,
    /// <summary>Division by zero.</summary>
    DISP_E_DIVBYZERO = 0x80020012,
    /// <summary>Buffer too small</summary>
    DISP_E_BUFFERTOOSMALL = 0x80020013,
    /// <summary>Buffer too small.</summary>
    TYPE_E_BUFFERTOOSMALL = 0x80028016,
    /// <summary>Field name not defined in the record.</summary>
    TYPE_E_FIELDNOTFOUND = 0x80028017,
    /// <summary>Old format or invalid type library.</summary>
    TYPE_E_INVDATAREAD = 0x80028018,
    /// <summary>Old format or invalid type library.</summary>
    TYPE_E_UNSUPFORMAT = 0x80028019,
    /// <summary>Error accessing the OLE registry.</summary>
    TYPE_E_REGISTRYACCESS = 0x8002801C,
    /// <summary>Library not registered.</summary>
    TYPE_E_LIBNOTREGISTERED = 0x8002801D,
    /// <summary>Bound to unknown type.</summary>
    TYPE_E_UNDEFINEDTYPE = 0x80028027,
    /// <summary>Qualified name disallowed.</summary>
    TYPE_E_QUALIFIEDNAMEDISALLOWED = 0x80028028,
    /// <summary>Invalid forward reference, or reference to uncompiled type.</summary>
    TYPE_E_INVALIDSTATE = 0x80028029,
    /// <summary>Type mismatch.</summary>
    TYPE_E_WRONGTYPEKIND = 0x8002802A,
    /// <summary>Element not found.</summary>
    TYPE_E_ELEMENTNOTFOUND = 0x8002802B,
    /// <summary>Ambiguous name.</summary>
    TYPE_E_AMBIGUOUSNAME = 0x8002802C,
    /// <summary>Name already exists in the library.</summary>
    TYPE_E_NAMECONFLICT = 0x8002802D,
    /// <summary>Unknown LCID.</summary>
    TYPE_E_UNKNOWNLCID = 0x8002802E,
    /// <summary>Function not defined in specified DLL.</summary>
    TYPE_E_DLLFUNCTIONNOTFOUND = 0x8002802F,
    /// <summary>Wrong module kind for the operation.</summary>
    TYPE_E_BADMODULEKIND = 0x800288BD,
    /// <summary>Size may not exceed 64K.</summary>
    TYPE_E_SIZETOOBIG = 0x800288C5,
    /// <summary>Duplicate ID in inheritance hierarchy.</summary>
    TYPE_E_DUPLICATEID = 0x800288C6,
    /// <summary>Incorrect inheritance depth in standard OLE hmember.</summary>
    TYPE_E_INVALIDID = 0x800288CF,
    /// <summary>Type mismatch.</summary>
    TYPE_E_TYPEMISMATCH = 0x80028CA0,
    /// <summary>Invalid number of arguments.</summary>
    TYPE_E_OUTOFBOUNDS = 0x80028CA1,
    /// <summary>I/O Error.</summary>
    TYPE_E_IOERROR = 0x80028CA2,
    /// <summary>Error creating unique tmp file.</summary>
    TYPE_E_CANTCREATETMPFILE = 0x80028CA3,
    /// <summary>Error loading type library/DLL.</summary>
    TYPE_E_CANTLOADLIBRARY = 0x80029C4A,
    /// <summary>Inconsistent property functions.</summary>
    TYPE_E_INCONSISTENTPROPFUNCS = 0x80029C83,
    /// <summary>Circular dependency between types/modules.</summary>
    TYPE_E_CIRCULARTYPE = 0x80029C84,
    /// <summary>Unable to perform requested operation.</summary>
    STG_E_INVALIDFUNCTION = 0x80030001,
    /// <summary>%1 could not be found.</summary>
    STG_E_FILENOTFOUND = 0x80030002,
    /// <summary>The path %1 could not be found.</summary>
    STG_E_PATHNOTFOUND = 0x80030003,
    /// <summary>There are insufficient resources to open another file.</summary>
    STG_E_TOOMANYOPENFILES = 0x80030004,
    /// <summary>Access Denied.</summary>
    STG_E_ACCESSDENIED = 0x80030005,
    /// <summary>Attempted an operation on an invalid object.</summary>
    STG_E_INVALIDHANDLE = 0x80030006,
    /// <summary>There is insufficient memory available to complete operation.</summary>
    STG_E_INSUFFICIENTMEMORY = 0x80030008,
    /// <summary>Invalid pointer error.</summary>
    STG_E_INVALIDPOINTER = 0x80030009,
    /// <summary>There are no more entries to return.</summary>
    STG_E_NOMOREFILES = 0x80030012,
    /// <summary>Disk is write-protected.</summary>
    STG_E_DISKISWRITEPROTECTED = 0x80030013,
    /// <summary>An error occurred during a seek operation.</summary>
    STG_E_SEEKERROR = 0x80030019,
    /// <summary>A disk error occurred during a write operation.</summary>
    STG_E_WRITEFAULT = 0x8003001D,
    /// <summary>A disk error occurred during a read operation.</summary>
    STG_E_READFAULT = 0x8003001E,
    /// <summary>A share violation has occurred.</summary>
    STG_E_SHAREVIOLATION = 0x80030020,
    /// <summary>A lock violation has occurred.</summary>
    STG_E_LOCKVIOLATION = 0x80030021,
    /// <summary>%1 already exists.</summary>
    STG_E_FILEALREADYEXISTS = 0x80030050,
    /// <summary>Invalid parameter error.</summary>
    STG_E_INVALIDPARAMETER = 0x80030057,
    /// <summary>There is insufficient disk space to complete operation.</summary>
    STG_E_MEDIUMFULL = 0x80030070,
    /// <summary>Illegal write of non-simple property to simple property set.</summary>
    STG_E_PROPSETMISMATCHED = 0x800300F0,
    /// <summary>An API call exited abnormally.</summary>
    STG_E_ABNORMALAPIEXIT = 0x800300FA,
    /// <summary>The file %1 is not a valid compound file.</summary>
    STG_E_INVALIDHEADER = 0x800300FB,
    /// <summary>The name %1 is not valid.</summary>
    STG_E_INVALIDNAME = 0x800300FC,
    /// <summary>An unexpected error occurred.</summary>
    STG_E_UNKNOWN = 0x800300FD,
    /// <summary>That function is not implemented.</summary>
    STG_E_UNIMPLEMENTEDFUNCTION = 0x800300FE,
    /// <summary>Invalid flag error.</summary>
    STG_E_INVALIDFLAG = 0x800300FF,
    /// <summary>Attempted to use an object that is busy.</summary>
    STG_E_INUSE = 0x80030100,
    /// <summary>The storage has been changed since the last commit.</summary>
    STG_E_NOTCURRENT = 0x80030101,
    /// <summary>Attempted to use an object that has ceased to exist.</summary>
    STG_E_REVERTED = 0x80030102,
    /// <summary>Can&#39;t save.</summary>
    STG_E_CANTSAVE = 0x80030103,
    /// <summary>The compound file %1 was produced with an incompatible version of storage.</summary>
    STG_E_OLDFORMAT = 0x80030104,
    /// <summary>The compound file %1 was produced with a newer version of storage.</summary>
    STG_E_OLDDLL = 0x80030105,
    /// <summary>Share.exe or equivalent is required for operation.</summary>
    STG_E_SHAREREQUIRED = 0x80030106,
    /// <summary>Illegal operation called on non-file based storage.</summary>
    STG_E_NOTFILEBASEDSTORAGE = 0x80030107,
    /// <summary>Illegal operation called on object with extant marshallings.</summary>
    STG_E_EXTANTMARSHALLINGS = 0x80030108,
    /// <summary>The docfile has been corrupted.</summary>
    STG_E_DOCFILECORRUPT = 0x80030109,
    /// <summary>OLE32.DLL has been loaded at the wrong address.</summary>
    STG_E_BADBASEADDRESS = 0x80030110,
    /// <summary>The compound file is too large for the current implementation</summary>
    STG_E_DOCFILETOOLARGE = 0x80030111,
    /// <summary>The compound file was not created with the STGM_SIMPLE flag</summary>
    STG_E_NOTSIMPLEFORMAT = 0x80030112,
    /// <summary>The file download was aborted abnormally. The file is incomplete.</summary>
    STG_E_INCOMPLETE = 0x80030201,
    /// <summary>The file download has been terminated.</summary>
    STG_E_TERMINATED = 0x80030202,
    /// <summary>The underlying file was converted to compound file format.</summary>
    STG_S_CONVERTED = 0x00030200,
    /// <summary>The storage operation should block until more data is available.</summary>
    STG_S_BLOCK = 0x00030201,
    /// <summary>The storage operation should retry immediately.</summary>
    STG_S_RETRYNOW = 0x00030202,
    /// <summary>The notified event sink will not influence the storage operation.</summary>
    STG_S_MONITORING = 0x00030203,
    /// <summary>Multiple opens prevent consolidated. (commit succeeded).</summary>
    STG_S_MULTIPLEOPENS = 0x00030204,
    /// <summary>Consolidation of the storage file failed. (commit succeeded).</summary>
    STG_S_CONSOLIDATIONFAILED = 0x00030205,
    /// <summary>Consolidation of the storage file is inappropriate. (commit succeeded).</summary>
    STG_S_CANNOTCONSOLIDATE = 0x00030206,
    /// <summary>Generic Copy Protection Error.</summary>
    STG_E_STATUS_COPY_PROTECTION_FAILURE = 0x80030305,
    /// <summary>Copy Protection Error - DVD CSS Authentication failed.</summary>
    STG_E_CSS_AUTHENTICATION_FAILURE = 0x80030306,
    /// <summary>Copy Protection Error - The given sector does not have a valid CSS key.</summary>
    STG_E_CSS_KEY_NOT_PRESENT = 0x80030307,
    /// <summary>Copy Protection Error - DVD session key not established.</summary>
    STG_E_CSS_KEY_NOT_ESTABLISHED = 0x80030308,
    /// <summary>Copy Protection Error - The read failed because the sector is encrypted.</summary>
    STG_E_CSS_SCRAMBLED_SECTOR = 0x80030309,
    /// <summary>Copy Protection Error - The current DVD&#39;s region does not correspond to the region setting of the drive.</summary>
    STG_E_CSS_REGION_MISMATCH = 0x8003030A,
    /// <summary>Copy Protection Error - The drive&#39;s region setting may be permanent or the number of user resets has been exhausted.</summary>
    STG_E_RESETS_EXHAUSTED = 0x8003030B,
    /// <summary>Call was rejected by callee.</summary>
    RPC_E_CALL_REJECTED = 0x80010001,
    /// <summary>Call was canceled by the message filter.</summary>
    RPC_E_CALL_CANCELED = 0x80010002,
    /// <summary>The caller is dispatching an intertask SendMessage call and cannot call out via PostMessage.</summary>
    RPC_E_CANTPOST_INSENDCALL = 0x80010003,
    /// <summary>The caller is dispatching an asynchronous call and cannot make an outgoing call on behalf of this call.</summary>
    RPC_E_CANTCALLOUT_INASYNCCALL = 0x80010004,
    /// <summary>It is illegal to call out while inside message filter.</summary>
    RPC_E_CANTCALLOUT_INEXTERNALCALL = 0x80010005,
    /// <summary>The connection terminated or is in a bogus state and cannot be used any more. Other connections are still valid.</summary>
    RPC_E_CONNECTION_TERMINATED = 0x80010006,
    /// <summary>The callee (server [not server application]) is not available and disappeared; all connections are invalid. The call may have executed.</summary>
    RPC_E_SERVER_DIED = 0x80010007,
    /// <summary>The caller (client) disappeared while the callee (server) was processing a call.</summary>
    RPC_E_CLIENT_DIED = 0x80010008,
    /// <summary>The data packet with the marshalled parameter data is incorrect.</summary>
    RPC_E_INVALID_DATAPACKET = 0x80010009,
    /// <summary>The call was not transmitted properly; the message queue was full and was not emptied after yielding.</summary>
    RPC_E_CANTTRANSMIT_CALL = 0x8001000A,
    /// <summary>The client (caller) cannot marshall the parameter data - low memory, etc.</summary>
    RPC_E_CLIENT_CANTMARSHAL_DATA = 0x8001000B,
    /// <summary>The client (caller) cannot unmarshall the return data - low memory, etc.</summary>
    RPC_E_CLIENT_CANTUNMARSHAL_DATA = 0x8001000C,
    /// <summary>The server (callee) cannot marshall the return data - low memory, etc.</summary>
    RPC_E_SERVER_CANTMARSHAL_DATA = 0x8001000D,
    /// <summary>The server (callee) cannot unmarshall the parameter data - low memory, etc.</summary>
    RPC_E_SERVER_CANTUNMARSHAL_DATA = 0x8001000E,
    /// <summary>Received data is invalid; could be server or client data.</summary>
    RPC_E_INVALID_DATA = 0x8001000F,
    /// <summary>A particular parameter is invalid and cannot be (un)marshalled.</summary>
    RPC_E_INVALID_PARAMETER = 0x80010010,
    /// <summary>There is no second outgoing call on same channel in DDE conversation.</summary>
    RPC_E_CANTCALLOUT_AGAIN = 0x80010011,
    /// <summary>The callee (server [not server application]) is not available and disappeared; all connections are invalid. The call did not execute.</summary>
    RPC_E_SERVER_DIED_DNE = 0x80010012,
    /// <summary>System call failed.</summary>
    RPC_E_SYS_CALL_FAILED = 0x80010100,
    /// <summary>Could not allocate some required resource (memory, events, ...)</summary>
    RPC_E_OUT_OF_RESOURCES = 0x80010101,
    /// <summary>Attempted to make calls on more than one thread in single threaded mode.</summary>
    RPC_E_ATTEMPTED_MULTITHREAD = 0x80010102,
    /// <summary>The requested interface is not registered on the server object.</summary>
    RPC_E_NOT_REGISTERED = 0x80010103,
    /// <summary>RPC could not call the server or could not return the results of calling the server.</summary>
    RPC_E_FAULT = 0x80010104,
    /// <summary>The server threw an exception.</summary>
    RPC_E_SERVERFAULT = 0x80010105,
    /// <summary>Cannot change thread mode after it is set.</summary>
    RPC_E_CHANGED_MODE = 0x80010106,
    /// <summary>The method called does not exist on the server.</summary>
    RPC_E_INVALIDMETHOD = 0x80010107,
    /// <summary>The object invoked has disconnected from its clients.</summary>
    RPC_E_DISCONNECTED = 0x80010108,
    /// <summary>The object invoked chose not to process the call now. Try again later.</summary>
    RPC_E_RETRY = 0x80010109,
    /// <summary>The message filter indicated that the application is busy.</summary>
    RPC_E_SERVERCALL_RETRYLATER = 0x8001010A,
    /// <summary>The message filter rejected the call.</summary>
    RPC_E_SERVERCALL_REJECTED = 0x8001010B,
    /// <summary>A call control interfaces was called with invalid data.</summary>
    RPC_E_INVALID_CALLDATA = 0x8001010C,
    /// <summary>An outgoing call cannot be made since the application is dispatching an input-synchronous call.</summary>
    RPC_E_CANTCALLOUT_ININPUTSYNCCALL = 0x8001010D,
    /// <summary>The application called an interface that was marshalled for a different thread.</summary>
    RPC_E_WRONG_THREAD = 0x8001010E,
    /// <summary>CoInitialize has not been called on the current thread.</summary>
    RPC_E_THREAD_NOT_INIT = 0x8001010F,
    /// <summary>The version of OLE on the client and server machines does not match.</summary>
    RPC_E_VERSION_MISMATCH = 0x80010110,
    /// <summary>OLE received a packet with an invalid header.</summary>
    RPC_E_INVALID_HEADER = 0x80010111,
    /// <summary>OLE received a packet with an invalid extension.</summary>
    RPC_E_INVALID_EXTENSION = 0x80010112,
    /// <summary>The requested object or interface does not exist.</summary>
    RPC_E_INVALID_IPID = 0x80010113,
    /// <summary>The requested object does not exist.</summary>
    RPC_E_INVALID_OBJECT = 0x80010114,
    /// <summary>OLE has sent a request and is waiting for a reply.</summary>
    RPC_S_CALLPENDING = 0x80010115,
    /// <summary>OLE is waiting before retrying a request.</summary>
    RPC_S_WAITONTIMER = 0x80010116,
    /// <summary>Call context cannot be accessed after call completed.</summary>
    RPC_E_CALL_COMPLETE = 0x80010117,
    /// <summary>Impersonate on unsecure calls is not supported.</summary>
    RPC_E_UNSECURE_CALL = 0x80010118,
    /// <summary>Security must be initialized before any interfaces are marshalled or unmarshalled. It cannot be changed once initialized.</summary>
    RPC_E_TOO_LATE = 0x80010119,
    /// <summary>No security packages are installed on this machine or the user is not logged on or there are no compatible security packages between the client and server.</summary>
    RPC_E_NO_GOOD_SECURITY_PACKAGES = 0x8001011A,
    /// <summary>Access is denied.</summary>
    RPC_E_ACCESS_DENIED = 0x8001011B,
    /// <summary>Remote calls are not allowed for this process.</summary>
    RPC_E_REMOTE_DISABLED = 0x8001011C,
    /// <summary>The marshaled interface data packet (OBJREF) has an invalid or unknown format.</summary>
    RPC_E_INVALID_OBJREF = 0x8001011D,
    /// <summary>No context is associated with this call. This happens for some custom marshalled calls and on the client side of the call.</summary>
    RPC_E_NO_CONTEXT = 0x8001011E,
    /// <summary>This operation returned because the timeout period expired.</summary>
    RPC_E_TIMEOUT = 0x8001011F,
    /// <summary>There are no synchronize objects to wait on.</summary>
    RPC_E_NO_SYNC = 0x80010120,
    /// <summary>Full subject issuer chain SSL principal name expected from the server.</summary>
    RPC_E_FULLSIC_REQUIRED = 0x80010121,
    /// <summary>Principal name is not a valid MSSTD name.</summary>
    RPC_E_INVALID_STD_NAME = 0x80010122,
    /// <summary>Unable to impersonate DCOM client</summary>
    CO_E_FAILEDTOIMPERSONATE = 0x80010123,
    /// <summary>Unable to obtain server&#39;s security context</summary>
    CO_E_FAILEDTOGETSECCTX = 0x80010124,
    /// <summary>Unable to open the access token of the current thread</summary>
    CO_E_FAILEDTOOPENTHREADTOKEN = 0x80010125,
    /// <summary>Unable to obtain user info from an access token</summary>
    CO_E_FAILEDTOGETTOKENINFO = 0x80010126,
    /// <summary>The client who called IAccessControl::IsAccessPermitted was not the trustee provided to the method</summary>
    CO_E_TRUSTEEDOESNTMATCHCLIENT = 0x80010127,
    /// <summary>Unable to obtain the client&#39;s security blanket</summary>
    CO_E_FAILEDTOQUERYCLIENTBLANKET = 0x80010128,
    /// <summary>Unable to set a discretionary ACL into a security descriptor</summary>
    CO_E_FAILEDTOSETDACL = 0x80010129,
    /// <summary>The system function, AccessCheck, returned false</summary>
    CO_E_ACCESSCHECKFAILED = 0x8001012A,
    /// <summary>Either NetAccessDel or NetAccessAdd returned an error code.</summary>
    CO_E_NETACCESSAPIFAILED = 0x8001012B,
    /// <summary>One of the trustee strings provided by the user did not conform to the &lt;Domain&gt;\&lt;Name&gt; syntax and it was not the &quot;*&quot; string</summary>
    CO_E_WRONGTRUSTEENAMESYNTAX = 0x8001012C,
    /// <summary>One of the security identifiers provided by the user was invalid</summary>
    CO_E_INVALIDSID = 0x8001012D,
    /// <summary>Unable to convert a wide character trustee string to a multibyte trustee string</summary>
    CO_E_CONVERSIONFAILED = 0x8001012E,
    /// <summary>Unable to find a security identifier that corresponds to a trustee string provided by the user</summary>
    CO_E_NOMATCHINGSIDFOUND = 0x8001012F,
    /// <summary>The system function, LookupAccountSID, failed</summary>
    CO_E_LOOKUPACCSIDFAILED = 0x80010130,
    /// <summary>Unable to find a trustee name that corresponds to a security identifier provided by the user</summary>
    CO_E_NOMATCHINGNAMEFOUND = 0x80010131,
    /// <summary>The system function, LookupAccountName, failed</summary>
    CO_E_LOOKUPACCNAMEFAILED = 0x80010132,
    /// <summary>Unable to set or reset a serialization handle</summary>
    CO_E_SETSERLHNDLFAILED = 0x80010133,
    /// <summary>Unable to obtain the Windows directory</summary>
    CO_E_FAILEDTOGETWINDIR = 0x80010134,
    /// <summary>Path too long</summary>
    CO_E_PATHTOOLONG = 0x80010135,
    /// <summary>Unable to generate a uuid.</summary>
    CO_E_FAILEDTOGENUUID = 0x80010136,
    /// <summary>Unable to create file</summary>
    CO_E_FAILEDTOCREATEFILE = 0x80010137,
    /// <summary>Unable to close a serialization handle or a file handle.</summary>
    CO_E_FAILEDTOCLOSEHANDLE = 0x80010138,
    /// <summary>The number of ACEs in an ACL exceeds the system limit.</summary>
    CO_E_EXCEEDSYSACLLIMIT = 0x80010139,
    /// <summary>Not all the DENY_ACCESS ACEs are arranged in front of the GRANT_ACCESS ACEs in the stream.</summary>
    CO_E_ACESINWRONGORDER = 0x8001013A,
    /// <summary>The version of ACL format in the stream is not supported by this implementation of IAccessControl</summary>
    CO_E_INCOMPATIBLESTREAMVERSION = 0x8001013B,
    /// <summary>Unable to open the access token of the server process</summary>
    CO_E_FAILEDTOOPENPROCESSTOKEN = 0x8001013C,
    /// <summary>Unable to decode the ACL in the stream provided by the user</summary>
    CO_E_DECODEFAILED = 0x8001013D,
    /// <summary>The COM IAccessControl object is not initialized</summary>
    CO_E_ACNOTINITIALIZED = 0x8001013F,
    /// <summary>Call Cancellation is disabled</summary>
    CO_E_CANCEL_DISABLED = 0x80010140,
    /// <summary>An internal error occurred.</summary>
    RPC_E_UNEXPECTED = 0x8001FFFF,
    /// <summary>The specified event is currently not being audited.</summary>
    ERROR_AUDITING_DISABLED = 0xC0090001,
    /// <summary>The SID filtering operation removed all SIDs.</summary>
    ERROR_ALL_SIDS_FILTERED = 0xC0090002,
    /// <summary>Business rule scripts are disabled for the calling application.</summary>
    ERROR_BIZRULES_NOT_ENABLED = 0xC0090003,
    /// <summary>Bad UID.</summary>
    NTE_BAD_UID = 0x80090001,
    /// <summary>Bad Hash.</summary>
    NTE_BAD_HASH = 0x80090002,
    /// <summary>Bad Key.</summary>
    NTE_BAD_KEY = 0x80090003,
    /// <summary>Bad Length.</summary>
    NTE_BAD_LEN = 0x80090004,
    /// <summary>Bad Data.</summary>
    NTE_BAD_DATA = 0x80090005,
    /// <summary>Invalid Signature.</summary>
    NTE_BAD_SIGNATURE = 0x80090006,
    /// <summary>Bad Version of provider.</summary>
    NTE_BAD_VER = 0x80090007,
    /// <summary>Invalid algorithm specified.</summary>
    NTE_BAD_ALGID = 0x80090008,
    /// <summary>Invalid flags specified.</summary>
    NTE_BAD_FLAGS = 0x80090009,
    /// <summary>Invalid type specified.</summary>
    NTE_BAD_TYPE = 0x8009000A,
    /// <summary>Key not valid for use in specified state.</summary>
    NTE_BAD_KEY_STATE = 0x8009000B,
    /// <summary>Hash not valid for use in specified state.</summary>
    NTE_BAD_HASH_STATE = 0x8009000C,
    /// <summary>Key does not exist.</summary>
    NTE_NO_KEY = 0x8009000D,
    /// <summary>Insufficient memory available for the operation.</summary>
    NTE_NO_MEMORY = 0x8009000E,
    /// <summary>Object already exists.</summary>
    NTE_EXISTS = 0x8009000F,
    /// <summary>Access denied.</summary>
    NTE_PERM = 0x80090010,
    /// <summary>Object was not found.</summary>
    NTE_NOT_FOUND = 0x80090011,
    /// <summary>Data already encrypted.</summary>
    NTE_DOUBLE_ENCRYPT = 0x80090012,
    /// <summary>Invalid provider specified.</summary>
    NTE_BAD_PROVIDER = 0x80090013,
    /// <summary>Invalid provider type specified.</summary>
    NTE_BAD_PROV_TYPE = 0x80090014,
    /// <summary>Provider&#39;s public key is invalid.</summary>
    NTE_BAD_PUBLIC_KEY = 0x80090015,
    /// <summary>Keyset does not exist</summary>
    NTE_BAD_KEYSET = 0x80090016,
    /// <summary>Provider type not defined.</summary>
    NTE_PROV_TYPE_NOT_DEF = 0x80090017,
    /// <summary>Provider type as registered is invalid.</summary>
    NTE_PROV_TYPE_ENTRY_BAD = 0x80090018,
    /// <summary>The keyset is not defined.</summary>
    NTE_KEYSET_NOT_DEF = 0x80090019,
    /// <summary>Keyset as registered is invalid.</summary>
    NTE_KEYSET_ENTRY_BAD = 0x8009001A,
    /// <summary>Provider type does not match registered value.</summary>
    NTE_PROV_TYPE_NO_MATCH = 0x8009001B,
    /// <summary>The digital signature file is corrupt.</summary>
    NTE_SIGNATURE_FILE_BAD = 0x8009001C,
    /// <summary>Provider DLL failed to initialize correctly.</summary>
    NTE_PROVIDER_DLL_FAIL = 0x8009001D,
    /// <summary>Provider DLL could not be found.</summary>
    NTE_PROV_DLL_NOT_FOUND = 0x8009001E,
    /// <summary>The Keyset parameter is invalid.</summary>
    NTE_BAD_KEYSET_PARAM = 0x8009001F,
    /// <summary>An internal error occurred.</summary>
    NTE_FAIL = 0x80090020,
    /// <summary>A base error occurred.</summary>
    NTE_SYS_ERR = 0x80090021,
    /// <summary>Provider could not perform the action since the context was acquired as silent.</summary>
    NTE_SILENT_CONTEXT = 0x80090022,
    /// <summary>The security token does not have storage space available for an additional container.</summary>
    NTE_TOKEN_KEYSET_STORAGE_FULL = 0x80090023,
    /// <summary>The profile for the user is a temporary profile.</summary>
    NTE_TEMPORARY_PROFILE = 0x80090024,
    /// <summary>The key parameters could not be set because the CSP uses fixed parameters.</summary>
    NTE_FIXEDPARAMETER = 0x80090025,
    /// <summary>The supplied handle is invalid.</summary>
    NTE_INVALID_HANDLE = 0x80090026,
    /// <summary>The parameter is incorrect.</summary>
    NTE_INVALID_PARAMETER = 0x80090027,
    /// <summary>The buffer supplied to a function was too small.</summary>
    NTE_BUFFER_TOO_SMALL = 0x80090028,
    /// <summary>The requested operation is not supported.</summary>
    NTE_NOT_SUPPORTED = 0x80090029,
    /// <summary>No more data is available.</summary>
    NTE_NO_MORE_ITEMS = 0x8009002A,
    /// <summary>The supplied buffers overlap incorrectly.</summary>
    NTE_BUFFERS_OVERLAP = 0x8009002B,
    /// <summary>The specified data could not be decrypted.</summary>
    NTE_DECRYPTION_FAILURE = 0x8009002C,
    /// <summary>An internal consistency check failed.</summary>
    NTE_INTERNAL_ERROR = 0x8009002D,
    /// <summary>This operation requires input from the user.</summary>
    NTE_UI_REQUIRED = 0x8009002E,
    /// <summary>The cryptographic provider does not support HMAC.</summary>
    NTE_HMAC_NOT_SUPPORTED = 0x8009002F,
    /// <summary>Not enough memory is available to complete this request</summary>
    SEC_E_INSUFFICIENT_MEMORY = 0x80090300,
    /// <summary>The handle specified is invalid</summary>
    SEC_E_INVALID_HANDLE = 0x80090301,
    /// <summary>The function requested is not supported</summary>
    SEC_E_UNSUPPORTED_FUNCTION = 0x80090302,
    /// <summary>The specified target is unknown or unreachable</summary>
    SEC_E_TARGET_UNKNOWN = 0x80090303,
    /// <summary>The Local Security Authority cannot be contacted</summary>
    SEC_E_INTERNAL_ERROR = 0x80090304,
    /// <summary>The requested security package does not exist</summary>
    SEC_E_SECPKG_NOT_FOUND = 0x80090305,
    /// <summary>The caller is not the owner of the desired credentials</summary>
    SEC_E_NOT_OWNER = 0x80090306,
    /// <summary>The security package failed to initialize, and cannot be installed</summary>
    SEC_E_CANNOT_INSTALL = 0x80090307,
    /// <summary>The token supplied to the function is invalid</summary>
    SEC_E_INVALID_TOKEN = 0x80090308,
    /// <summary>The security package is not able to marshall the logon buffer, so the logon attempt has failed</summary>
    SEC_E_CANNOT_PACK = 0x80090309,
    /// <summary>The per-message Quality of Protection is not supported by the security package</summary>
    SEC_E_QOP_NOT_SUPPORTED = 0x8009030A,
    /// <summary>The security context does not allow impersonation of the client</summary>
    SEC_E_NO_IMPERSONATION = 0x8009030B,
    /// <summary>The logon attempt failed</summary>
    SEC_E_LOGON_DENIED = 0x8009030C,
    /// <summary>The credentials supplied to the package were not recognized</summary>
    SEC_E_UNKNOWN_CREDENTIALS = 0x8009030D,
    /// <summary>No credentials are available in the security package</summary>
    SEC_E_NO_CREDENTIALS = 0x8009030E,
    /// <summary>The message or signature supplied for verification has been altered</summary>
    SEC_E_MESSAGE_ALTERED = 0x8009030F,
    /// <summary>The message supplied for verification is out of sequence</summary>
    SEC_E_OUT_OF_SEQUENCE = 0x80090310,
    /// <summary>No authority could be contacted for authentication.</summary>
    SEC_E_NO_AUTHENTICATING_AUTHORITY = 0x80090311,
    /// <summary>The function completed successfully, but must be called again to complete the context</summary>
    SEC_I_CONTINUE_NEEDED = 0x00090312,
    /// <summary>The function completed successfully, but CompleteToken must be called</summary>
    SEC_I_COMPLETE_NEEDED = 0x00090313,
    /// <summary>The function completed successfully, but both CompleteToken and this function must be called to complete the context</summary>
    SEC_I_COMPLETE_AND_CONTINUE = 0x00090314,
    /// <summary>The logon was completed, but no network authority was available. The logon was made using locally known information</summary>
    SEC_I_LOCAL_LOGON = 0x00090315,
    /// <summary>The requested security package does not exist</summary>
    SEC_E_BAD_PKGID = 0x80090316,
    /// <summary>The context has expired and can no longer be used.</summary>
    SEC_E_CONTEXT_EXPIRED = 0x80090317,
    /// <summary>The context has expired and can no longer be used.</summary>
    SEC_I_CONTEXT_EXPIRED = 0x00090317,
    /// <summary>The supplied message is incomplete. The signature was not verified.</summary>
    SEC_E_INCOMPLETE_MESSAGE = 0x80090318,
    /// <summary>The credentials supplied were not complete, and could not be verified. The context could not be initialized.</summary>
    SEC_E_INCOMPLETE_CREDENTIALS = 0x80090320,
    /// <summary>The buffers supplied to a function was too small.</summary>
    SEC_E_BUFFER_TOO_SMALL = 0x80090321,
    /// <summary>The credentials supplied were not complete, and could not be verified. Additional information can be returned from the context.</summary>
    SEC_I_INCOMPLETE_CREDENTIALS = 0x00090320,
    /// <summary>The context data must be renegotiated with the peer.</summary>
    SEC_I_RENEGOTIATE = 0x00090321,
    /// <summary>The target principal name is incorrect.</summary>
    SEC_E_WRONG_PRINCIPAL = 0x80090322,
    /// <summary>There is no LSA mode context associated with this context.</summary>
    SEC_I_NO_LSA_CONTEXT = 0x00090323,
    /// <summary>The clocks on the client and server machines are skewed.</summary>
    SEC_E_TIME_SKEW = 0x80090324,
    /// <summary>The certificate chain was issued by an authority that is not trusted.</summary>
    SEC_E_UNTRUSTED_ROOT = 0x80090325,
    /// <summary>The message received was unexpected or badly formatted.</summary>
    SEC_E_ILLEGAL_MESSAGE = 0x80090326,
    /// <summary>An unknown error occurred while processing the certificate.</summary>
    SEC_E_CERT_UNKNOWN = 0x80090327,
    /// <summary>The received certificate has expired.</summary>
    SEC_E_CERT_EXPIRED = 0x80090328,
    /// <summary>The specified data could not be encrypted.</summary>
    SEC_E_ENCRYPT_FAILURE = 0x80090329,
    /// <summary>The specified data could not be decrypted.</summary>
    SEC_E_DECRYPT_FAILURE = 0x80090330,
    /// <summary>The client and server cannot communicate, because they do not possess a common algorithm.</summary>
    SEC_E_ALGORITHM_MISMATCH = 0x80090331,
    /// <summary>The security context could not be established due to a failure in the requested quality of service (e.g. mutual authentication or delegation).</summary>
    SEC_E_SECURITY_QOS_FAILED = 0x80090332,
    /// <summary>A security context was deleted before the context was completed. This is considered a logon failure.</summary>
    SEC_E_UNFINISHED_CONTEXT_DELETED = 0x80090333,
    /// <summary>The client is trying to negotiate a context and the server requires user-to-user but didn&#39;t send a TGT reply.</summary>
    SEC_E_NO_TGT_REPLY = 0x80090334,
    /// <summary>Unable to accomplish the requested task because the local machine does not have any IP addresses.</summary>
    SEC_E_NO_IP_ADDRESSES = 0x80090335,
    /// <summary>The supplied credential handle does not match the credential associated with the security context.</summary>
    SEC_E_WRONG_CREDENTIAL_HANDLE = 0x80090336,
    /// <summary>The crypto system or checksum function is invalid because a required function is unavailable.</summary>
    SEC_E_CRYPTO_SYSTEM_INVALID = 0x80090337,
    /// <summary>The number of maximum ticket referrals has been exceeded.</summary>
    SEC_E_MAX_REFERRALS_EXCEEDED = 0x80090338,
    /// <summary>The local machine must be a Kerberos KDC (domain controller) and it is not.</summary>
    SEC_E_MUST_BE_KDC = 0x80090339,
    /// <summary>The other end of the security negotiation is requires strong crypto but it is not supported on the local machine.</summary>
    SEC_E_STRONG_CRYPTO_NOT_SUPPORTED = 0x8009033A,
    /// <summary>The KDC reply contained more than one principal name.</summary>
    SEC_E_TOO_MANY_PRINCIPALS = 0x8009033B,
    /// <summary>Expected to find PA data for a hint of what etype to use, but it was not found.</summary>
    SEC_E_NO_PA_DATA = 0x8009033C,
    /// <summary>The client certificate does not contain a valid UPN, or does not match the client name in the logon request. Please contact your administrator.</summary>
    SEC_E_PKINIT_NAME_MISMATCH = 0x8009033D,
    /// <summary>Smartcard logon is required and was not used.</summary>
    SEC_E_SMARTCARD_LOGON_REQUIRED = 0x8009033E,
    /// <summary>A system shutdown is in progress.</summary>
    SEC_E_SHUTDOWN_IN_PROGRESS = 0x8009033F,
    /// <summary>An invalid request was sent to the KDC.</summary>
    SEC_E_KDC_INVALID_REQUEST = 0x80090340,
    /// <summary>The KDC was unable to generate a referral for the service requested.</summary>
    SEC_E_KDC_UNABLE_TO_REFER = 0x80090341,
    /// <summary>The encryption type requested is not supported by the KDC.</summary>
    SEC_E_KDC_UNKNOWN_ETYPE = 0x80090342,
    /// <summary>An unsupported preauthentication mechanism was presented to the Kerberos package.</summary>
    SEC_E_UNSUPPORTED_PREAUTH = 0x80090343,
    /// <summary>The requested operation cannot be completed. The computer must be trusted for delegation and the current user account must be configured to allow delegation.</summary>
    SEC_E_DELEGATION_REQUIRED = 0x80090345,
    /// <summary>Client&#39;s supplied SSPI channel bindings were incorrect.</summary>
    SEC_E_BAD_BINDINGS = 0x80090346,
    /// <summary>The received certificate was mapped to multiple accounts.</summary>
    SEC_E_MULTIPLE_ACCOUNTS = 0x80090347,
    /// <summary> SEC_E_NO_KERB_KEY</summary>
    SEC_E_NO_KERB_KEY = 0x80090348,
    /// <summary>The certificate is not valid for the requested usage.</summary>
    SEC_E_CERT_WRONG_USAGE = 0x80090349,
    /// <summary>The system detected a possible attempt to compromise security. Please ensure that you can contact the server that authenticated you.</summary>
    SEC_E_DOWNGRADE_DETECTED = 0x80090350,
    /// <summary>The smartcard certificate used for authentication has been revoked. Please contact your system administrator. There may be additional information in the event log.</summary>
    SEC_E_SMARTCARD_CERT_REVOKED = 0x80090351,
    /// <summary>An untrusted certificate authority was detected While processing the smartcard certificate used for authentication. Please contact your system administrator.</summary>
    SEC_E_ISSUING_CA_UNTRUSTED = 0x80090352,
    /// <summary>The revocation status of the smartcard certificate used for authentication could not be determined. Please contact your system administrator.</summary>
    SEC_E_REVOCATION_OFFLINE_C = 0x80090353,
    /// <summary>The smartcard certificate used for authentication was not trusted. Please contact your system administrator.</summary>
    SEC_E_PKINIT_CLIENT_FAILURE = 0x80090354,
    /// <summary>The smartcard certificate used for authentication has expired. Please contact your system administrator.</summary>
    SEC_E_SMARTCARD_CERT_EXPIRED = 0x80090355,
    /// <summary>The Kerberos subsystem encountered an error. A service for user protocol request was made against a domain controller which does not support service for user.</summary>
    SEC_E_NO_S4U_PROT_SUPPORT = 0x80090356,
    /// <summary>An attempt was made by this server to make a Kerberos constrained delegation request for a target outside of the server&#39;s realm. This is not supported, and indicates a misconfiguration on this server&#39;s allowed to delegate to list. Please contact your administrator.</summary>
    SEC_E_CROSSREALM_DELEGATION_FAILURE = 0x80090357,
    /// <summary>The revocation status of the domain controller certificate used for smartcard authentication could not be determined. There is additional information in the system event log. Please contact your system administrator.</summary>
    SEC_E_REVOCATION_OFFLINE_KDC = 0x80090358,
    /// <summary>An untrusted certificate authority was detected while processing the domain controller certificate used for authentication. There is additional information in the system event log. Please contact your system administrator.</summary>
    SEC_E_ISSUING_CA_UNTRUSTED_KDC = 0x80090359,
    /// <summary>The domain controller certificate used for smartcard logon has expired. Please contact your system administrator with the contents of your system event log.</summary>
    SEC_E_KDC_CERT_EXPIRED = 0x8009035A,
    /// <summary>The domain controller certificate used for smartcard logon has been revoked. Please contact your system administrator with the contents of your system event log.</summary>
    SEC_E_KDC_CERT_REVOKED = 0x8009035B,
    /// <summary>A signature operation must be performed before the user can authenticate.</summary>
    SEC_I_SIGNATURE_NEEDED = 0x0009035C,
    /// <summary>One or more of the parameters passed to the function was invalid.</summary>
    SEC_E_INVALID_PARAMETER = 0x8009035D,
    /// <summary>Client policy does not allow credential delegation to target server.</summary>
    SEC_E_DELEGATION_POLICY = 0x8009035E,
    /// <summary>Client policy does not allow credential delegation to target server with NLTM only authentication.</summary>
    SEC_E_POLICY_NLTM_ONLY = 0x8009035F,
    /// <summary>The recipient rejected the renegotiation request.</summary>
    SEC_I_NO_RENEGOTIATION = 0x00090360,
    /// <summary>The required security context does not exist.</summary>
    SEC_E_NO_CONTEXT = 0x80090361,
    /// <summary>The PKU2U protocol encountered an error while attempting to utilize the associated certificates.</summary>
    SEC_E_PKU2U_CERT_FAILURE = 0x80090362,
    /// <summary>The identity of the server computer could not be verified.</summary>
    SEC_E_MUTUAL_AUTH_FAILED = 0x80090363,
    /// <summary>An error occurred while performing an operation on a cryptographic message.</summary>
    CRYPT_E_MSG_ERROR = 0x80091001,
    /// <summary>Unknown cryptographic algorithm.</summary>
    CRYPT_E_UNKNOWN_ALGO = 0x80091002,
    /// <summary>The object identifier is poorly formatted.</summary>
    CRYPT_E_OID_FORMAT = 0x80091003,
    /// <summary>Invalid cryptographic message type.</summary>
    CRYPT_E_INVALID_MSG_TYPE = 0x80091004,
    /// <summary>Unexpected cryptographic message encoding.</summary>
    CRYPT_E_UNEXPECTED_ENCODING = 0x80091005,
    /// <summary>The cryptographic message does not contain an expected authenticated attribute.</summary>
    CRYPT_E_AUTH_ATTR_MISSING = 0x80091006,
    /// <summary>The hash value is not correct.</summary>
    CRYPT_E_HASH_VALUE = 0x80091007,
    /// <summary>The index value is not valid.</summary>
    CRYPT_E_INVALID_INDEX = 0x80091008,
    /// <summary>The content of the cryptographic message has already been decrypted.</summary>
    CRYPT_E_ALREADY_DECRYPTED = 0x80091009,
    /// <summary>The content of the cryptographic message has not been decrypted yet.</summary>
    CRYPT_E_NOT_DECRYPTED = 0x8009100A,
    /// <summary>The enveloped-data message does not contain the specified recipient.</summary>
    CRYPT_E_RECIPIENT_NOT_FOUND = 0x8009100B,
    /// <summary>Invalid control type.</summary>
    CRYPT_E_CONTROL_TYPE = 0x8009100C,
    /// <summary>Invalid issuer and/or serial number.</summary>
    CRYPT_E_ISSUER_SERIALNUMBER = 0x8009100D,
    /// <summary>Cannot find the original signer.</summary>
    CRYPT_E_SIGNER_NOT_FOUND = 0x8009100E,
    /// <summary>The cryptographic message does not contain all of the requested attributes.</summary>
    CRYPT_E_ATTRIBUTES_MISSING = 0x8009100F,
    /// <summary>The streamed cryptographic message is not ready to return data.</summary>
    CRYPT_E_STREAM_MSG_NOT_READY = 0x80091010,
    /// <summary>The streamed cryptographic message requires more data to complete the decode operation.</summary>
    CRYPT_E_STREAM_INSUFFICIENT_DATA = 0x80091011,
    /// <summary>The protected data needs to be re-protected.</summary>
    CRYPT_I_NEW_PROTECTION_REQUIRED = 0x00091012,
    /// <summary>The length specified for the output data was insufficient.</summary>
    CRYPT_E_BAD_LEN = 0x80092001,
    /// <summary>An error occurred during encode or decode operation.</summary>
    CRYPT_E_BAD_ENCODE = 0x80092002,
    /// <summary>An error occurred while reading or writing to a file.</summary>
    CRYPT_E_FILE_ERROR = 0x80092003,
    /// <summary>Cannot find object or property.</summary>
    CRYPT_E_NOT_FOUND = 0x80092004,
    /// <summary>The object or property already exists.</summary>
    CRYPT_E_EXISTS = 0x80092005,
    /// <summary>No provider was specified for the store or object.</summary>
    CRYPT_E_NO_PROVIDER = 0x80092006,
    /// <summary>The specified certificate is self signed.</summary>
    CRYPT_E_SELF_SIGNED = 0x80092007,
    /// <summary>The previous certificate or CRL context was deleted.</summary>
    CRYPT_E_DELETED_PREV = 0x80092008,
    /// <summary>Cannot find the requested object.</summary>
    CRYPT_E_NO_MATCH = 0x80092009,
    /// <summary>The certificate does not have a property that references a private key.</summary>
    CRYPT_E_UNEXPECTED_MSG_TYPE = 0x8009200A,
    /// <summary>Cannot find the certificate and private key for decryption.</summary>
    CRYPT_E_NO_KEY_PROPERTY = 0x8009200B,
    /// <summary>Cannot find the certificate and private key to use for decryption.</summary>
    CRYPT_E_NO_DECRYPT_CERT = 0x8009200C,
    /// <summary>Not a cryptographic message or the cryptographic message is not formatted correctly.</summary>
    CRYPT_E_BAD_MSG = 0x8009200D,
    /// <summary>The signed cryptographic message does not have a signer for the specified signer index.</summary>
    CRYPT_E_NO_SIGNER = 0x8009200E,
    /// <summary>Final closure is pending until additional frees or closes.</summary>
    CRYPT_E_PENDING_CLOSE = 0x8009200F,
    /// <summary>The certificate is revoked.</summary>
    CRYPT_E_REVOKED = 0x80092010,
    /// <summary>No Dll or exported function was found to verify revocation.</summary>
    CRYPT_E_NO_REVOCATION_DLL = 0x80092011,
    /// <summary>The revocation function was unable to check revocation for the certificate.</summary>
    CRYPT_E_NO_REVOCATION_CHECK = 0x80092012,
    /// <summary>The revocation function was unable to check revocation because the revocation server was offline.</summary>
    CRYPT_E_REVOCATION_OFFLINE = 0x80092013,
    /// <summary>The certificate is not in the revocation server&#39;s database.</summary>
    CRYPT_E_NOT_IN_REVOCATION_DATABASE = 0x80092014,
    /// <summary>The string contains a non-numeric character.</summary>
    CRYPT_E_INVALID_NUMERIC_STRING = 0x80092020,
    /// <summary>The string contains a non-printable character.</summary>
    CRYPT_E_INVALID_PRINTABLE_STRING = 0x80092021,
    /// <summary>The string contains a character not in the 7 bit ASCII character set.</summary>
    CRYPT_E_INVALID_IA5_STRING = 0x80092022,
    /// <summary>The string contains an invalid X500 name attribute key, oid, value or delimiter.</summary>
    CRYPT_E_INVALID_X500_STRING = 0x80092023,
    /// <summary>The dwValueType for the CERT_NAME_VALUE is not one of the character strings. Most likely it is either a CERT_RDN_ENCODED_BLOB or CERT_TDN_OCTED_STRING.</summary>
    CRYPT_E_NOT_CHAR_STRING = 0x80092024,
    /// <summary>The Put operation cannot continue. The file needs to be resized. However, there is already a signature present. A complete signing operation must be done.</summary>
    CRYPT_E_FILERESIZED = 0x80092025,
    /// <summary>The cryptographic operation failed due to a local security option setting.</summary>
    CRYPT_E_SECURITY_SETTINGS = 0x80092026,
    /// <summary>No DLL or exported function was found to verify subject usage.</summary>
    CRYPT_E_NO_VERIFY_USAGE_DLL = 0x80092027,
    /// <summary>The called function was unable to do a usage check on the subject.</summary>
    CRYPT_E_NO_VERIFY_USAGE_CHECK = 0x80092028,
    /// <summary>Since the server was offline, the called function was unable to complete the usage check.</summary>
    CRYPT_E_VERIFY_USAGE_OFFLINE = 0x80092029,
    /// <summary>The subject was not found in a Certificate Trust List (CTL).</summary>
    CRYPT_E_NOT_IN_CTL = 0x8009202A,
    /// <summary>None of the signers of the cryptographic message or certificate trust list is trusted.</summary>
    CRYPT_E_NO_TRUSTED_SIGNER = 0x8009202B,
    /// <summary>The public key&#39;s algorithm parameters are missing.</summary>
    CRYPT_E_MISSING_PUBKEY_PARA = 0x8009202C,
    /// <summary>OSS Certificate encode/decode error code base See asn1code.h for a definition of the OSS runtime errors. The OSS error values are offset by CRYPT_E_OSS_ERROR.</summary>
    CRYPT_E_OSS_ERROR = 0x80093000,
    /// <summary>OSS ASN.1 Error: Output Buffer is too small.</summary>
    OSS_MORE_BUF = 0x80093001,
    /// <summary>OSS ASN.1 Error: Signed integer is encoded as a unsigned integer.</summary>
    OSS_NEGATIVE_UINTEGER = 0x80093002,
    /// <summary>OSS ASN.1 Error: Unknown ASN.1 data type.</summary>
    OSS_PDU_RANGE = 0x80093003,
    /// <summary>OSS ASN.1 Error: Output buffer is too small, the decoded data has been truncated.</summary>
    OSS_MORE_INPUT = 0x80093004,
    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    OSS_DATA_ERROR = 0x80093005,
    /// <summary>OSS ASN.1 Error: Invalid argument.</summary>
    OSS_BAD_ARG = 0x80093006,
    /// <summary>OSS ASN.1 Error: Encode/Decode version mismatch.</summary>
    OSS_BAD_VERSION = 0x80093007,
    /// <summary>OSS ASN.1 Error: Out of memory.</summary>
    OSS_OUT_MEMORY = 0x80093008,
    /// <summary>OSS ASN.1 Error: Encode/Decode Error.</summary>
    OSS_PDU_MISMATCH = 0x80093009,
    /// <summary>OSS ASN.1 Error: Internal Error.</summary>
    OSS_LIMITED = 0x8009300A,
    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    OSS_BAD_PTR = 0x8009300B,
    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    OSS_BAD_TIME = 0x8009300C,
    /// <summary>OSS ASN.1 Error: Unsupported BER indefinite-length encoding.</summary>
    OSS_INDEFINITE_NOT_SUPPORTED = 0x8009300D,
    /// <summary>OSS ASN.1 Error: Access violation.</summary>
    OSS_MEM_ERROR = 0x8009300E,
    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    OSS_BAD_TABLE = 0x8009300F,
    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    OSS_TOO_LONG = 0x80093010,
    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    OSS_CONSTRAINT_VIOLATED = 0x80093011,
    /// <summary>OSS ASN.1 Error: Internal Error.</summary>
    OSS_FATAL_ERROR = 0x80093012,
    /// <summary>OSS ASN.1 Error: Multi-threading conflict.</summary>
    OSS_ACCESS_SERIALIZATION_ERROR = 0x80093013,
    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    OSS_NULL_TBL = 0x80093014,
    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    OSS_NULL_FCN = 0x80093015,
    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    OSS_BAD_ENCRULES = 0x80093016,
    /// <summary>OSS ASN.1 Error: Encode/Decode function not implemented.</summary>
    OSS_UNAVAIL_ENCRULES = 0x80093017,
    /// <summary>OSS ASN.1 Error: Trace file error.</summary>
    OSS_CANT_OPEN_TRACE_WINDOW = 0x80093018,
    /// <summary>OSS ASN.1 Error: Function not implemented.</summary>
    OSS_UNIMPLEMENTED = 0x80093019,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_OID_DLL_NOT_LINKED = 0x8009301A,
    /// <summary>OSS ASN.1 Error: Trace file error.</summary>
    OSS_CANT_OPEN_TRACE_FILE = 0x8009301B,
    /// <summary>OSS ASN.1 Error: Trace file error.</summary>
    OSS_TRACE_FILE_ALREADY_OPEN = 0x8009301C,
    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    OSS_TABLE_MISMATCH = 0x8009301D,
    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    OSS_TYPE_NOT_SUPPORTED = 0x8009301E,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_REAL_DLL_NOT_LINKED = 0x8009301F,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_REAL_CODE_NOT_LINKED = 0x80093020,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_OUT_OF_RANGE = 0x80093021,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_COPIER_DLL_NOT_LINKED = 0x80093022,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_CONSTRAINT_DLL_NOT_LINKED = 0x80093023,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_COMPARATOR_DLL_NOT_LINKED = 0x80093024,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_COMPARATOR_CODE_NOT_LINKED = 0x80093025,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_MEM_MGR_DLL_NOT_LINKED = 0x80093026,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_PDV_DLL_NOT_LINKED = 0x80093027,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_PDV_CODE_NOT_LINKED = 0x80093028,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_API_DLL_NOT_LINKED = 0x80093029,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_BERDER_DLL_NOT_LINKED = 0x8009302A,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_PER_DLL_NOT_LINKED = 0x8009302B,
    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    OSS_OPEN_TYPE_ERROR = 0x8009302C,
    /// <summary>OSS ASN.1 Error: System resource error.</summary>
    OSS_MUTEX_NOT_CREATED = 0x8009302D,
    /// <summary>OSS ASN.1 Error: Trace file error.</summary>
    OSS_CANT_CLOSE_TRACE_FILE = 0x8009302E,
    /// <summary>ASN1 Certificate encode/decode error code base. The ASN1 error values are offset by CRYPT_E_ASN1_ERROR.</summary>
    CRYPT_E_ASN1_ERROR = 0x80093100,
    /// <summary>ASN1 internal encode or decode error.</summary>
    CRYPT_E_ASN1_INTERNAL = 0x80093101,
    /// <summary>ASN1 unexpected end of data.</summary>
    CRYPT_E_ASN1_EOD = 0x80093102,
    /// <summary>ASN1 corrupted data.</summary>
    CRYPT_E_ASN1_CORRUPT = 0x80093103,
    /// <summary>ASN1 value too large.</summary>
    CRYPT_E_ASN1_LARGE = 0x80093104,
    /// <summary>ASN1 constraint violated.</summary>
    CRYPT_E_ASN1_CONSTRAINT = 0x80093105,
    /// <summary>ASN1 out of memory.</summary>
    CRYPT_E_ASN1_MEMORY = 0x80093106,
    /// <summary>ASN1 buffer overflow.</summary>
    CRYPT_E_ASN1_OVERFLOW = 0x80093107,
    /// <summary>ASN1 function not supported for this PDU.</summary>
    CRYPT_E_ASN1_BADPDU = 0x80093108,
    /// <summary>ASN1 bad arguments to function call.</summary>
    CRYPT_E_ASN1_BADARGS = 0x80093109,
    /// <summary>ASN1 bad real value.</summary>
    CRYPT_E_ASN1_BADREAL = 0x8009310A,
    /// <summary>ASN1 bad tag value met.</summary>
    CRYPT_E_ASN1_BADTAG = 0x8009310B,
    /// <summary>ASN1 bad choice value.</summary>
    CRYPT_E_ASN1_CHOICE = 0x8009310C,
    /// <summary>ASN1 bad encoding rule.</summary>
    CRYPT_E_ASN1_RULE = 0x8009310D,
    /// <summary>ASN1 bad unicode (UTF8).</summary>
    CRYPT_E_ASN1_UTF8 = 0x8009310E,
    /// <summary>ASN1 bad PDU type.</summary>
    CRYPT_E_ASN1_PDU_TYPE = 0x80093133,
    /// <summary>ASN1 not yet implemented.</summary>
    CRYPT_E_ASN1_NYI = 0x80093134,
    /// <summary>ASN1 skipped unknown extension(s).</summary>
    CRYPT_E_ASN1_EXTENDED = 0x80093201,
    /// <summary>ASN1 end of data expected</summary>
    CRYPT_E_ASN1_NOEOD = 0x80093202,
    /// <summary>The request subject name is invalid or too long.</summary>
    CERTSRV_E_BAD_REQUESTSUBJECT = 0x80094001,
    /// <summary>The request does not exist.</summary>
    CERTSRV_E_NO_REQUEST = 0x80094002,
    /// <summary>The request&#39;s current status does not allow this operation.</summary>
    CERTSRV_E_BAD_REQUESTSTATUS = 0x80094003,
    /// <summary>The requested property value is empty.</summary>
    CERTSRV_E_PROPERTY_EMPTY = 0x80094004,
    /// <summary>The certification authority&#39;s certificate contains invalid data.</summary>
    CERTSRV_E_INVALID_CA_CERTIFICATE = 0x80094005,
    /// <summary>Certificate service has been suspended for a database restore operation.</summary>
    CERTSRV_E_SERVER_SUSPENDED = 0x80094006,
    /// <summary>The certificate contains an encoded length that is potentially incompatible with older enrollment software.</summary>
    CERTSRV_E_ENCODING_LENGTH = 0x80094007,
    /// <summary>The operation is denied. The user has multiple roles assigned and the certification authority is configured to enforce role separation.</summary>
    CERTSRV_E_ROLECONFLICT = 0x80094008,
    /// <summary>The operation is denied. It can only be performed by a certificate manager that is allowed to manage certificates for the current requester.</summary>
    CERTSRV_E_RESTRICTEDOFFICER = 0x80094009,
    /// <summary>Cannot archive private key. The certification authority is not configured for key archival.</summary>
    CERTSRV_E_KEY_ARCHIVAL_NOT_CONFIGURED = 0x8009400A,
    /// <summary>Cannot archive private key. The certification authority could not verify one or more key recovery certificates.</summary>
    CERTSRV_E_NO_VALID_KRA = 0x8009400B,
    /// <summary>The request is incorrectly formatted. The encrypted private key must be in an unauthenticated attribute in an outermost signature.</summary>
    CERTSRV_E_BAD_REQUEST_KEY_ARCHIVAL = 0x8009400C,
    /// <summary>At least one security principal must have the permission to manage this CA.</summary>
    CERTSRV_E_NO_CAADMIN_DEFINED = 0x8009400D,
    /// <summary>The request contains an invalid renewal certificate attribute.</summary>
    CERTSRV_E_BAD_RENEWAL_CERT_ATTRIBUTE = 0x8009400E,
    /// <summary>An attempt was made to open a Certification Authority database session, but there are already too many active sessions. The server may need to be configured to allow additional sessions.</summary>
    CERTSRV_E_NO_DB_SESSIONS = 0x8009400F,
    /// <summary>A memory reference caused a data alignment fault.</summary>
    CERTSRV_E_ALIGNMENT_FAULT = 0x80094010,
    /// <summary>The permissions on this certification authority do not allow the current user to enroll for certificates.</summary>
    CERTSRV_E_ENROLL_DENIED = 0x80094011,
    /// <summary>The permissions on the certificate template do not allow the current user to enroll for this type of certificate.</summary>
    CERTSRV_E_TEMPLATE_DENIED = 0x80094012,
    /// <summary>The contacted domain controller cannot support signed LDAP traffic. Update the domain controller or configure Certificate Services to use SSL for Active Directory access.</summary>
    CERTSRV_E_DOWNLEVEL_DC_SSL_OR_UPGRADE = 0x80094013,
    /// <summary>The request was denied by a certificate manager or CA administrator.</summary>
    CERTSRV_E_ADMIN_DENIED_REQUEST = 0x80094014,
    /// <summary>An enrollment policy server cannot be located.</summary>
    CERTSRV_E_NO_POLICY_SERVER = 0x80094015,
    /// <summary>The requested certificate template is not supported by this CA.</summary>
    CERTSRV_E_UNSUPPORTED_CERT_TYPE = 0x80094800,
    /// <summary>The request contains no certificate template information.</summary>
    CERTSRV_E_NO_CERT_TYPE = 0x80094801,
    /// <summary>The request contains conflicting template information.</summary>
    CERTSRV_E_TEMPLATE_CONFLICT = 0x80094802,
    /// <summary>The request is missing a required Subject Alternate name extension.</summary>
    CERTSRV_E_SUBJECT_ALT_NAME_REQUIRED = 0x80094803,
    /// <summary>The request is missing a required private key for archival by the server.</summary>
    CERTSRV_E_ARCHIVED_KEY_REQUIRED = 0x80094804,
    /// <summary>The request is missing a required SMIME capabilities extension.</summary>
    CERTSRV_E_SMIME_REQUIRED = 0x80094805,
    /// <summary>The request was made on behalf of a subject other than the caller. The certificate template must be configured to require at least one signature to authorize the request.</summary>
    CERTSRV_E_BAD_RENEWAL_SUBJECT = 0x80094806,
    /// <summary>The request template version is newer than the supported template version.</summary>
    CERTSRV_E_BAD_TEMPLATE_VERSION = 0x80094807,
    /// <summary>The template is missing a required signature policy attribute.</summary>
    CERTSRV_E_TEMPLATE_POLICY_REQUIRED = 0x80094808,
    /// <summary>The request is missing required signature policy information.</summary>
    CERTSRV_E_SIGNATURE_POLICY_REQUIRED = 0x80094809,
    /// <summary>The request is missing one or more required signatures.</summary>
    CERTSRV_E_SIGNATURE_COUNT = 0x8009480A,
    /// <summary>One or more signatures did not include the required application or issuance policies. The request is missing one or more required valid signatures.</summary>
    CERTSRV_E_SIGNATURE_REJECTED = 0x8009480B,
    /// <summary>The request is missing one or more required signature issuance policies.</summary>
    CERTSRV_E_ISSUANCE_POLICY_REQUIRED = 0x8009480C,
    /// <summary>The UPN is unavailable and cannot be added to the Subject Alternate name.</summary>
    CERTSRV_E_SUBJECT_UPN_REQUIRED = 0x8009480D,
    /// <summary>The Active Directory GUID is unavailable and cannot be added to the Subject Alternate name.</summary>
    CERTSRV_E_SUBJECT_DIRECTORY_GUID_REQUIRED = 0x8009480E,
    /// <summary>The DNS name is unavailable and cannot be added to the Subject Alternate name.</summary>
    CERTSRV_E_SUBJECT_DNS_REQUIRED = 0x8009480F,
    /// <summary>The request includes a private key for archival by the server, but key archival is not enabled for the specified certificate template.</summary>
    CERTSRV_E_ARCHIVED_KEY_UNEXPECTED = 0x80094810,
    /// <summary>The public key does not meet the minimum size required by the specified certificate template.</summary>
    CERTSRV_E_KEY_LENGTH = 0x80094811,
    /// <summary>The EMail name is unavailable and cannot be added to the Subject or Subject Alternate name.</summary>
    CERTSRV_E_SUBJECT_EMAIL_REQUIRED = 0x80094812,
    /// <summary>One or more certificate templates to be enabled on this certification authority could not be found.</summary>
    CERTSRV_E_UNKNOWN_CERT_TYPE = 0x80094813,
    /// <summary>The certificate template renewal period is longer than the certificate validity period. The template should be reconfigured or the CA certificate renewed.</summary>
    CERTSRV_E_CERT_TYPE_OVERLAP = 0x80094814,
    /// <summary>The certificate template requires too many RA signatures. Only one RA signature is allowed.</summary>
    CERTSRV_E_TOO_MANY_SIGNATURES = 0x80094815,
    /// <summary>The key is not exportable.</summary>
    XENROLL_E_KEY_NOT_EXPORTABLE = 0x80095000,
    /// <summary>You cannot add the root CA certificate into your local store.</summary>
    XENROLL_E_CANNOT_ADD_ROOT_CERT = 0x80095001,
    /// <summary>The key archival hash attribute was not found in the response.</summary>
    XENROLL_E_RESPONSE_KA_HASH_NOT_FOUND = 0x80095002,
    /// <summary>An unexpected key archival hash attribute was found in the response.</summary>
    XENROLL_E_RESPONSE_UNEXPECTED_KA_HASH = 0x80095003,
    /// <summary>There is a key archival hash mismatch between the request and the response.</summary>
    XENROLL_E_RESPONSE_KA_HASH_MISMATCH = 0x80095004,
    /// <summary>Signing certificate cannot include SMIME extension.</summary>
    XENROLL_E_KEYSPEC_SMIME_MISMATCH = 0x80095005,
    /// <summary>A system-level error occurred while verifying trust.</summary>
    TRUST_E_SYSTEM_ERROR = 0x80096001,
    /// <summary>The certificate for the signer of the message is invalid or not found.</summary>
    TRUST_E_NO_SIGNER_CERT = 0x80096002,
    /// <summary>One of the counter signatures was invalid.</summary>
    TRUST_E_COUNTER_SIGNER = 0x80096003,
    /// <summary>The signature of the certificate cannot be verified.</summary>
    TRUST_E_CERT_SIGNATURE = 0x80096004,
    /// <summary>The timestamp signature and/or certificate could not be verified or is malformed.</summary>
    TRUST_E_TIME_STAMP = 0x80096005,
    /// <summary>The digital signature of the object did not verify.</summary>
    TRUST_E_BAD_DIGEST = 0x80096010,
    /// <summary>A certificate&#39;s basic constraint extension has not been observed.</summary>
    TRUST_E_BASIC_CONSTRAINTS = 0x80096019,
    /// <summary>The certificate does not meet or contain the Authenticode(tm) financial extensions.</summary>
    TRUST_E_FINANCIAL_CRITERIA = 0x8009601E,
    /// <summary>Tried to reference a part of the file outside the proper range.</summary>
    MSSIPOTF_E_OUTOFMEMRANGE = 0x80097001,
    /// <summary>Could not retrieve an object from the file.</summary>
    MSSIPOTF_E_CANTGETOBJECT = 0x80097002,
    /// <summary>Could not find the head table in the file.</summary>
    MSSIPOTF_E_NOHEADTABLE = 0x80097003,
    /// <summary>The magic number in the head table is incorrect.</summary>
    MSSIPOTF_E_BAD_MAGICNUMBER = 0x80097004,
    /// <summary>The offset table has incorrect values.</summary>
    MSSIPOTF_E_BAD_OFFSET_TABLE = 0x80097005,
    /// <summary>Duplicate table tags or tags out of alphabetical order.</summary>
    MSSIPOTF_E_TABLE_TAGORDER = 0x80097006,
    /// <summary>A table does not start on a long word boundary.</summary>
    MSSIPOTF_E_TABLE_LONGWORD = 0x80097007,
    /// <summary>First table does not appear after header information.</summary>
    MSSIPOTF_E_BAD_FIRST_TABLE_PLACEMENT = 0x80097008,
    /// <summary>Two or more tables overlap.</summary>
    MSSIPOTF_E_TABLES_OVERLAP = 0x80097009,
    /// <summary>Too many pad bytes between tables or pad bytes are not 0.</summary>
    MSSIPOTF_E_TABLE_PADBYTES = 0x8009700A,
    /// <summary>File is too small to contain the last table.</summary>
    MSSIPOTF_E_FILETOOSMALL = 0x8009700B,
    /// <summary>A table checksum is incorrect.</summary>
    MSSIPOTF_E_TABLE_CHECKSUM = 0x8009700C,
    /// <summary>The file checksum is incorrect.</summary>
    MSSIPOTF_E_FILE_CHECKSUM = 0x8009700D,
    /// <summary>The signature does not have the correct attributes for the policy.</summary>
    MSSIPOTF_E_FAILED_POLICY = 0x80097010,
    /// <summary>The file did not pass the hints check.</summary>
    MSSIPOTF_E_FAILED_HINTS_CHECK = 0x80097011,
    /// <summary>The file is not an OpenType file.</summary>
    MSSIPOTF_E_NOT_OPENTYPE = 0x80097012,
    /// <summary>Failed on a file operation (open, map, read, write).</summary>
    MSSIPOTF_E_FILE = 0x80097013,
    /// <summary>A call to a CryptoAPI function failed.</summary>
    MSSIPOTF_E_CRYPT = 0x80097014,
    /// <summary>There is a bad version number in the file.</summary>
    MSSIPOTF_E_BADVERSION = 0x80097015,
    /// <summary>The structure of the DSIG table is incorrect.</summary>
    MSSIPOTF_E_DSIG_STRUCTURE = 0x80097016,
    /// <summary>A check failed in a partially constant table.</summary>
    MSSIPOTF_E_PCONST_CHECK = 0x80097017,
    /// <summary>Some kind of structural error.</summary>
    MSSIPOTF_E_STRUCTURE = 0x80097018,
    /// <summary>The requested credential requires confirmation.</summary>
    ERROR_CRED_REQUIRES_CONFIRMATION = 0x80097019,
    /// <summary>Unknown trust provider.</summary>
    TRUST_E_PROVIDER_UNKNOWN = 0x800B0001,
    /// <summary>The trust verification action specified is not supported by the specified trust provider.</summary>
    TRUST_E_ACTION_UNKNOWN = 0x800B0002,
    /// <summary>The form specified for the subject is not one supported or known by the specified trust provider.</summary>
    TRUST_E_SUBJECT_FORM_UNKNOWN = 0x800B0003,
    /// <summary>The subject is not trusted for the specified action.</summary>
    TRUST_E_SUBJECT_NOT_TRUSTED = 0x800B0004,
    /// <summary>Error due to problem in ASN.1 encoding process.</summary>
    DIGSIG_E_ENCODE = 0x800B0005,
    /// <summary>Error due to problem in ASN.1 decoding process.</summary>
    DIGSIG_E_DECODE = 0x800B0006,
    /// <summary>Reading / writing Extensions where Attributes are appropriate, and visa versa.</summary>
    DIGSIG_E_EXTENSIBILITY = 0x800B0007,
    /// <summary>Unspecified cryptographic failure.</summary>
    DIGSIG_E_CRYPTO = 0x800B0008,
    /// <summary>The size of the data could not be determined.</summary>
    PERSIST_E_SIZEDEFINITE = 0x800B0009,
    /// <summary>The size of the indefinite-sized data could not be determined.</summary>
    PERSIST_E_SIZEINDEFINITE = 0x800B000A,
    /// <summary>This object does not read and write self-sizing data.</summary>
    PERSIST_E_NOTSELFSIZING = 0x800B000B,
    /// <summary>No signature was present in the subject.</summary>
    TRUST_E_NOSIGNATURE = 0x800B0100,
    /// <summary>A required certificate is not within its validity period when verifying against the current system clock or the timestamp in the signed file.</summary>
    CERT_E_EXPIRED = 0x800B0101,
    /// <summary>The validity periods of the certification chain do not nest correctly.</summary>
    CERT_E_VALIDITYPERIODNESTING = 0x800B0102,
    /// <summary>A certificate that can only be used as an end-entity is being used as a CA or visa versa.</summary>
    CERT_E_ROLE = 0x800B0103,
    /// <summary>A path length constraint in the certification chain has been violated.</summary>
    CERT_E_PATHLENCONST = 0x800B0104,
    /// <summary>A certificate contains an unknown extension that is marked &#39;critical&#39;.</summary>
    CERT_E_CRITICAL = 0x800B0105,
    /// <summary>A certificate being used for a purpose other than the ones specified by its CA.</summary>
    CERT_E_PURPOSE = 0x800B0106,
    /// <summary>A parent of a given certificate in fact did not issue that child certificate.</summary>
    CERT_E_ISSUERCHAINING = 0x800B0107,
    /// <summary>A certificate is missing or has an empty value for an important field, such as a subject or issuer name.</summary>
    CERT_E_MALFORMED = 0x800B0108,
    /// <summary>A certificate chain processed, but terminated in a root certificate which is not trusted by the trust provider.</summary>
    CERT_E_UNTRUSTEDROOT = 0x800B0109,
    /// <summary>A certificate chain could not be built to a trusted root authority.</summary>
    CERT_E_CHAINING = 0x800B010A,
    /// <summary>Generic trust failure.</summary>
    TRUST_E_FAIL = 0x800B010B,
    /// <summary>A certificate was explicitly revoked by its issuer.</summary>
    CERT_E_REVOKED = 0x800B010C,
    /// <summary>The certification path terminates with the test root which is not trusted with the current policy settings.</summary>
    CERT_E_UNTRUSTEDTESTROOT = 0x800B010D,
    /// <summary>The revocation process could not continue - the certificate(s) could not be checked.</summary>
    CERT_E_REVOCATION_FAILURE = 0x800B010E,
    /// <summary>The certificate&#39;s CN name does not match the passed value.</summary>
    CERT_E_CN_NO_MATCH = 0x800B010F,
    /// <summary>The certificate is not valid for the requested usage.</summary>
    CERT_E_WRONG_USAGE = 0x800B0110,
    /// <summary>The certificate was explicitly marked as untrusted by the user.</summary>
    TRUST_E_EXPLICIT_DISTRUST = 0x800B0111,
    /// <summary>A certification chain processed correctly, but one of the CA certificates is not trusted by the policy provider.</summary>
    CERT_E_UNTRUSTEDCA = 0x800B0112,
    /// <summary>The certificate has invalid policy.</summary>
    CERT_E_INVALID_POLICY = 0x800B0113,
    /// <summary>The certificate has an invalid name. The name is not included in the permitted list or is explicitly excluded.</summary>
    CERT_E_INVALID_NAME = 0x800B0114,
    /// <summary>A non-empty line was encountered in the INF before the start of a section.</summary>
    SPAPI_E_EXPECTED_SECTION_NAME = 0x800F0000,
    /// <summary>A section name marker in the INF is not complete, or does not exist on a line by itself.</summary>
    SPAPI_E_BAD_SECTION_NAME_LINE = 0x800F0001,
    /// <summary>An INF section was encountered whose name exceeds the maximum section name length.</summary>
    SPAPI_E_SECTION_NAME_TOO_LONG = 0x800F0002,
    /// <summary>The syntax of the INF is invalid.</summary>
    SPAPI_E_GENERAL_SYNTAX = 0x800F0003,
    /// <summary>The style of the INF is different than what was requested.</summary>
    SPAPI_E_WRONG_INF_STYLE = 0x800F0100,
    /// <summary>The required section was not found in the INF.</summary>
    SPAPI_E_SECTION_NOT_FOUND = 0x800F0101,
    /// <summary>The required line was not found in the INF.</summary>
    SPAPI_E_LINE_NOT_FOUND = 0x800F0102,
    /// <summary>The files affected by the installation of this file queue have not been backed up for uninstall.</summary>
    SPAPI_E_NO_BACKUP = 0x800F0103,
    /// <summary>The INF or the device information set or element does not have an associated install class.</summary>
    SPAPI_E_NO_ASSOCIATED_CLASS = 0x800F0200,
    /// <summary>The INF or the device information set or element does not match the specified install class.</summary>
    SPAPI_E_CLASS_MISMATCH = 0x800F0201,
    /// <summary>An existing device was found that is a duplicate of the device being manually installed.</summary>
    SPAPI_E_DUPLICATE_FOUND = 0x800F0202,
    /// <summary>There is no driver selected for the device information set or element.</summary>
    SPAPI_E_NO_DRIVER_SELECTED = 0x800F0203,
    /// <summary>The requested device registry key does not exist.</summary>
    SPAPI_E_KEY_DOES_NOT_EXIST = 0x800F0204,
    /// <summary>The device instance name is invalid.</summary>
    SPAPI_E_INVALID_DEVINST_NAME = 0x800F0205,
    /// <summary>The install class is not present or is invalid.</summary>
    SPAPI_E_INVALID_CLASS = 0x800F0206,
    /// <summary>The device instance cannot be created because it already exists.</summary>
    SPAPI_E_DEVINST_ALREADY_EXISTS = 0x800F0207,
    /// <summary>The operation cannot be performed on a device information element that has not been registered.</summary>
    SPAPI_E_DEVINFO_NOT_REGISTERED = 0x800F0208,
    /// <summary>The device property code is invalid.</summary>
    SPAPI_E_INVALID_REG_PROPERTY = 0x800F0209,
    /// <summary>The INF from which a driver list is to be built does not exist.</summary>
    SPAPI_E_NO_INF = 0x800F020A,
    /// <summary>The device instance does not exist in the hardware tree.</summary>
    SPAPI_E_NO_SUCH_DEVINST = 0x800F020B,
    /// <summary>The icon representing this install class cannot be loaded.</summary>
    SPAPI_E_CANT_LOAD_CLASS_ICON = 0x800F020C,
    /// <summary>The class installer registry entry is invalid.</summary>
    SPAPI_E_INVALID_CLASS_INSTALLER = 0x800F020D,
    /// <summary>The class installer has indicated that the default action should be performed for this installation request.</summary>
    SPAPI_E_DI_DO_DEFAULT = 0x800F020E,
    /// <summary>The operation does not require any files to be copied.</summary>
    SPAPI_E_DI_NOFILECOPY = 0x800F020F,
    /// <summary>The specified hardware profile does not exist.</summary>
    SPAPI_E_INVALID_HWPROFILE = 0x800F0210,
    /// <summary>There is no device information element currently selected for this device information set.</summary>
    SPAPI_E_NO_DEVICE_SELECTED = 0x800F0211,
    /// <summary>The operation cannot be performed because the device information set is locked.</summary>
    SPAPI_E_DEVINFO_LIST_LOCKED = 0x800F0212,
    /// <summary>The operation cannot be performed because the device information element is locked.</summary>
    SPAPI_E_DEVINFO_DATA_LOCKED = 0x800F0213,
    /// <summary>The specified path does not contain any applicable device INFs.</summary>
    SPAPI_E_DI_BAD_PATH = 0x800F0214,
    /// <summary>No class installer parameters have been set for the device information set or element.</summary>
    SPAPI_E_NO_CLASSINSTALL_PARAMS = 0x800F0215,
    /// <summary>The operation cannot be performed because the file queue is locked.</summary>
    SPAPI_E_FILEQUEUE_LOCKED = 0x800F0216,
    /// <summary>A service installation section in this INF is invalid.</summary>
    SPAPI_E_BAD_SERVICE_INSTALLSECT = 0x800F0217,
    /// <summary>There is no class driver list for the device information element.</summary>
    SPAPI_E_NO_CLASS_DRIVER_LIST = 0x800F0218,
    /// <summary>The installation failed because a function driver was not specified for this device instance.</summary>
    SPAPI_E_NO_ASSOCIATED_SERVICE = 0x800F0219,
    /// <summary>There is presently no default device interface designated for this interface class.</summary>
    SPAPI_E_NO_DEFAULT_DEVICE_INTERFACE = 0x800F021A,
    /// <summary>The operation cannot be performed because the device interface is currently active.</summary>
    SPAPI_E_DEVICE_INTERFACE_ACTIVE = 0x800F021B,
    /// <summary>The operation cannot be performed because the device interface has been removed from the system.</summary>
    SPAPI_E_DEVICE_INTERFACE_REMOVED = 0x800F021C,
    /// <summary>An interface installation section in this INF is invalid.</summary>
    SPAPI_E_BAD_INTERFACE_INSTALLSECT = 0x800F021D,
    /// <summary>This interface class does not exist in the system.</summary>
    SPAPI_E_NO_SUCH_INTERFACE_CLASS = 0x800F021E,
    /// <summary>The reference string supplied for this interface device is invalid.</summary>
    SPAPI_E_INVALID_REFERENCE_STRING = 0x800F021F,
    /// <summary>The specified machine name does not conform to UNC naming conventions.</summary>
    SPAPI_E_INVALID_MACHINENAME = 0x800F0220,
    /// <summary>A general remote communication error occurred.</summary>
    SPAPI_E_REMOTE_COMM_FAILURE = 0x800F0221,
    /// <summary>The machine selected for remote communication is not available at this time.</summary>
    SPAPI_E_MACHINE_UNAVAILABLE = 0x800F0222,
    /// <summary>The Plug and Play service is not available on the remote machine.</summary>
    SPAPI_E_NO_CONFIGMGR_SERVICES = 0x800F0223,
    /// <summary>The property page provider registry entry is invalid.</summary>
    SPAPI_E_INVALID_PROPPAGE_PROVIDER = 0x800F0224,
    /// <summary>The requested device interface is not present in the system.</summary>
    SPAPI_E_NO_SUCH_DEVICE_INTERFACE = 0x800F0225,
    /// <summary>The device&#39;s co-installer has additional work to perform after installation is complete.</summary>
    SPAPI_E_DI_POSTPROCESSING_REQUIRED = 0x800F0226,
    /// <summary>The device&#39;s co-installer is invalid.</summary>
    SPAPI_E_INVALID_COINSTALLER = 0x800F0227,
    /// <summary>There are no compatible drivers for this device.</summary>
    SPAPI_E_NO_COMPAT_DRIVERS = 0x800F0228,
    /// <summary>There is no icon that represents this device or device type.</summary>
    SPAPI_E_NO_DEVICE_ICON = 0x800F0229,
    /// <summary>A logical configuration specified in this INF is invalid.</summary>
    SPAPI_E_INVALID_INF_LOGCONFIG = 0x800F022A,
    /// <summary>The class installer has denied the request to install or upgrade this device.</summary>
    SPAPI_E_DI_DONT_INSTALL = 0x800F022B,
    /// <summary>One of the filter drivers installed for this device is invalid.</summary>
    SPAPI_E_INVALID_FILTER_DRIVER = 0x800F022C,
    /// <summary>The driver selected for this device does not support this version of Windows.</summary>
    SPAPI_E_NON_WINDOWS_NT_DRIVER = 0x800F022D,
    /// <summary>The driver selected for this device does not support Windows.</summary>
    SPAPI_E_NON_WINDOWS_DRIVER = 0x800F022E,
    /// <summary>The third-party INF does not contain digital signature information.</summary>
    SPAPI_E_NO_CATALOG_FOR_OEM_INF = 0x800F022F,
    /// <summary>An invalid attempt was made to use a device installation file queue for verification of digital signatures relative to other platforms.</summary>
    SPAPI_E_DEVINSTALL_QUEUE_NONNATIVE = 0x800F0230,
    /// <summary>The device cannot be disabled.</summary>
    SPAPI_E_NOT_DISABLEABLE = 0x800F0231,
    /// <summary>The device could not be dynamically removed.</summary>
    SPAPI_E_CANT_REMOVE_DEVINST = 0x800F0232,
    /// <summary>Cannot copy to specified target.</summary>
    SPAPI_E_INVALID_TARGET = 0x800F0233,
    /// <summary>Driver is not intended for this platform.</summary>
    SPAPI_E_DRIVER_NONNATIVE = 0x800F0234,
    /// <summary>Operation not allowed in WOW64.</summary>
    SPAPI_E_IN_WOW64 = 0x800F0235,
    /// <summary>The operation involving unsigned file copying was rolled back, so that a system restore point could be set.</summary>
    SPAPI_E_SET_SYSTEM_RESTORE_POINT = 0x800F0236,
    /// <summary>An INF was copied into the Windows INF directory in an improper manner.</summary>
    SPAPI_E_INCORRECTLY_COPIED_INF = 0x800F0237,
    /// <summary>The Security Configuration Editor (SCE) APIs have been disabled on this Embedded product.</summary>
    SPAPI_E_SCE_DISABLED = 0x800F0238,
    /// <summary>An unknown exception was encountered.</summary>
    SPAPI_E_UNKNOWN_EXCEPTION = 0x800F0239,
    /// <summary>A problem was encountered when accessing the Plug and Play registry database.</summary>
    SPAPI_E_PNP_REGISTRY_ERROR = 0x800F023A,
    /// <summary>The requested operation is not supported for a remote machine.</summary>
    SPAPI_E_REMOTE_REQUEST_UNSUPPORTED = 0x800F023B,
    /// <summary>The specified file is not an installed OEM INF.</summary>
    SPAPI_E_NOT_AN_INSTALLED_OEM_INF = 0x800F023C,
    /// <summary>One or more devices are presently installed using the specified INF.</summary>
    SPAPI_E_INF_IN_USE_BY_DEVICES = 0x800F023D,
    /// <summary>The requested device install operation is obsolete.</summary>
    SPAPI_E_DI_FUNCTION_OBSOLETE = 0x800F023E,
    /// <summary>A file could not be verified because it does not have an associated catalog signed via Authenticode(tm).</summary>
    SPAPI_E_NO_AUTHENTICODE_CATALOG = 0x800F023F,
    /// <summary>Authenticode(tm) signature verification is not supported for the specified INF.</summary>
    SPAPI_E_AUTHENTICODE_DISALLOWED = 0x800F0240,
    /// <summary>The INF was signed with an Authenticode(tm) catalog from a trusted publisher.</summary>
    SPAPI_E_AUTHENTICODE_TRUSTED_PUBLISHER = 0x800F0241,
    /// <summary>The publisher of an Authenticode(tm) signed catalog has not yet been established as trusted.</summary>
    SPAPI_E_AUTHENTICODE_TRUST_NOT_ESTABLISHED = 0x800F0242,
    /// <summary>The publisher of an Authenticode(tm) signed catalog was not established as trusted.</summary>
    SPAPI_E_AUTHENTICODE_PUBLISHER_NOT_TRUSTED = 0x800F0243,
    /// <summary>The software was tested for compliance with Windows Logo requirements on a different version of Windows, and may not be compatible with this version.</summary>
    SPAPI_E_SIGNATURE_OSATTRIBUTE_MISMATCH = 0x800F0244,
    /// <summary>The file may only be validated by a catalog signed via Authenticode(tm).</summary>
    SPAPI_E_ONLY_VALIDATE_VIA_AUTHENTICODE = 0x800F0245,
    /// <summary>One of the installers for this device cannot perform the installation at this time.</summary>
    SPAPI_E_DEVICE_INSTALLER_NOT_READY = 0x800F0246,
    /// <summary>A problem was encountered while attempting to add the driver to the store.</summary>
    SPAPI_E_DRIVER_STORE_ADD_FAILED = 0x800F0247,
    /// <summary>The installation of this device is forbidden by system policy. Contact your system administrator.</summary>
    SPAPI_E_DEVICE_INSTALL_BLOCKED = 0x800F0248,
    /// <summary>The installation of this driver is forbidden by system policy. Contact your system administrator.</summary>
    SPAPI_E_DRIVER_INSTALL_BLOCKED = 0x800F0249,
    /// <summary>The specified INF is the wrong type for this operation.</summary>
    SPAPI_E_WRONG_INF_TYPE = 0x800F024A,
    /// <summary>The hash for the file is not present in the specified catalog file. The file is likely corrupt or the victim of tampering.</summary>
    SPAPI_E_FILE_HASH_NOT_IN_CATALOG = 0x800F024B,
    /// <summary>A problem was encountered while attempting to delete the driver from the store.</summary>
    SPAPI_E_DRIVER_STORE_DELETE_FAILED = 0x800F024C,
    /// <summary>An unrecoverable stack overflow was encountered.</summary>
    SPAPI_E_UNRECOVERABLE_STACK_OVERFLOW = 0x800F0300,
    /// <summary>No installed components were detected.</summary>
    SPAPI_E_ERROR_NOT_INSTALLED = 0x800F1000,
    /// <summary>An internal consistency check failed.</summary>
    SCARD_F_INTERNAL_ERROR = 0x80100001,
    /// <summary>The action was cancelled by an SCardCancel request.</summary>
    SCARD_E_CANCELLED = 0x80100002,
    /// <summary>The supplied handle was invalid.</summary>
    SCARD_E_INVALID_HANDLE = 0x80100003,
    /// <summary>One or more of the supplied parameters could not be properly interpreted.</summary>
    SCARD_E_INVALID_PARAMETER = 0x80100004,
    /// <summary>Registry startup information is missing or invalid.</summary>
    SCARD_E_INVALID_TARGET = 0x80100005,
    /// <summary>Not enough memory available to complete this command.</summary>
    SCARD_E_NO_MEMORY = 0x80100006,
    /// <summary>An internal consistency timer has expired.</summary>
    SCARD_F_WAITED_TOO_LONG = 0x80100007,
    /// <summary>The data buffer to receive returned data is too small for the returned data.</summary>
    SCARD_E_INSUFFICIENT_BUFFER = 0x80100008,
    /// <summary>The specified reader name is not recognized.</summary>
    SCARD_E_UNKNOWN_READER = 0x80100009,
    /// <summary>The user-specified timeout value has expired.</summary>
    SCARD_E_TIMEOUT = 0x8010000A,
    /// <summary>The smart card cannot be accessed because of other connections outstanding.</summary>
    SCARD_E_SHARING_VIOLATION = 0x8010000B,
    /// <summary>The operation requires a Smart Card, but no Smart Card is currently in the device.</summary>
    SCARD_E_NO_SMARTCARD = 0x8010000C,
    /// <summary>The specified smart card name is not recognized.</summary>
    SCARD_E_UNKNOWN_CARD = 0x8010000D,
    /// <summary>The system could not dispose of the media in the requested manner.</summary>
    SCARD_E_CANT_DISPOSE = 0x8010000E,
    /// <summary>The requested protocols are incompatible with the protocol currently in use with the smart card.</summary>
    SCARD_E_PROTO_MISMATCH = 0x8010000F,
    /// <summary>The reader or smart card is not ready to accept commands.</summary>
    SCARD_E_NOT_READY = 0x80100010,
    /// <summary>One or more of the supplied parameters values could not be properly interpreted.</summary>
    SCARD_E_INVALID_VALUE = 0x80100011,
    /// <summary>The action was cancelled by the system, presumably to log off or shut down.</summary>
    SCARD_E_SYSTEM_CANCELLED = 0x80100012,
    /// <summary>An internal communications error has been detected.</summary>
    SCARD_F_COMM_ERROR = 0x80100013,
    /// <summary>An internal error has been detected, but the source is unknown.</summary>
    SCARD_F_UNKNOWN_ERROR = 0x80100014,
    /// <summary>An ATR obtained from the registry is not a valid ATR string.</summary>
    SCARD_E_INVALID_ATR = 0x80100015,
    /// <summary>An attempt was made to end a non-existent transaction.</summary>
    SCARD_E_NOT_TRANSACTED = 0x80100016,
    /// <summary>The specified reader is not currently available for use.</summary>
    SCARD_E_READER_UNAVAILABLE = 0x80100017,
    /// <summary>The operation has been aborted to allow the server application to exit.</summary>
    SCARD_P_SHUTDOWN = 0x80100018,
    /// <summary>The PCI Receive buffer was too small.</summary>
    SCARD_E_PCI_TOO_SMALL = 0x80100019,
    /// <summary>The reader driver does not meet minimal requirements for support.</summary>
    SCARD_E_READER_UNSUPPORTED = 0x8010001A,
    /// <summary>The reader driver did not produce a unique reader name.</summary>
    SCARD_E_DUPLICATE_READER = 0x8010001B,
    /// <summary>The smart card does not meet minimal requirements for support.</summary>
    SCARD_E_CARD_UNSUPPORTED = 0x8010001C,
    /// <summary>The Smart card resource manager is not running.</summary>
    SCARD_E_NO_SERVICE = 0x8010001D,
    /// <summary>The Smart card resource manager has shut down.</summary>
    SCARD_E_SERVICE_STOPPED = 0x8010001E,
    /// <summary>An unexpected card error has occurred.</summary>
    SCARD_E_UNEXPECTED = 0x8010001F,
    /// <summary>No Primary Provider can be found for the smart card.</summary>
    SCARD_E_ICC_INSTALLATION = 0x80100020,
    /// <summary>The requested order of object creation is not supported.</summary>
    SCARD_E_ICC_CREATEORDER = 0x80100021,
    /// <summary>This smart card does not support the requested feature.</summary>
    SCARD_E_UNSUPPORTED_FEATURE = 0x80100022,
    /// <summary>The identified directory does not exist in the smart card.</summary>
    SCARD_E_DIR_NOT_FOUND = 0x80100023,
    /// <summary>The identified file does not exist in the smart card.</summary>
    SCARD_E_FILE_NOT_FOUND = 0x80100024,
    /// <summary>The supplied path does not represent a smart card directory.</summary>
    SCARD_E_NO_DIR = 0x80100025,
    /// <summary>The supplied path does not represent a smart card file.</summary>
    SCARD_E_NO_FILE = 0x80100026,
    /// <summary>Access is denied to this file.</summary>
    SCARD_E_NO_ACCESS = 0x80100027,
    /// <summary>The smartcard does not have enough memory to store the information.</summary>
    SCARD_E_WRITE_TOO_MANY = 0x80100028,
    /// <summary>There was an error trying to set the smart card file object pointer.</summary>
    SCARD_E_BAD_SEEK = 0x80100029,
    /// <summary>The supplied PIN is incorrect.</summary>
    SCARD_E_INVALID_CHV = 0x8010002A,
    /// <summary>An unrecognized error code was returned from a layered component.</summary>
    SCARD_E_UNKNOWN_RES_MNG = 0x8010002B,
    /// <summary>The requested certificate does not exist.</summary>
    SCARD_E_NO_SUCH_CERTIFICATE = 0x8010002C,
    /// <summary>The requested certificate could not be obtained.</summary>
    SCARD_E_CERTIFICATE_UNAVAILABLE = 0x8010002D,
    /// <summary>Cannot find a smart card reader.</summary>
    SCARD_E_NO_READERS_AVAILABLE = 0x8010002E,
    /// <summary>A communications error with the smart card has been detected. Retry the operation.</summary>
    SCARD_E_COMM_DATA_LOST = 0x8010002F,
    /// <summary>The requested key container does not exist on the smart card.</summary>
    SCARD_E_NO_KEY_CONTAINER = 0x80100030,
    /// <summary>The Smart card resource manager is too busy to complete this operation.</summary>
    SCARD_E_SERVER_TOO_BUSY = 0x80100031,
    /// <summary>The smart card PIN cache has expired.</summary>
    SCARD_E_PIN_CACHE_EXPIRED = 0x80100032,
    /// <summary>The smart card PIN cannot be cached.</summary>
    SCARD_E_NO_PIN_CACHE = 0x80100033,
    /// <summary>The smart card is read only and cannot be written to.</summary>
    SCARD_E_READ_ONLY_CARD = 0x80100034,
    /// <summary>The reader cannot communicate with the smart card, due to ATR configuration conflicts.</summary>
    SCARD_W_UNSUPPORTED_CARD = 0x80100065,
    /// <summary>The smart card is not responding to a reset.</summary>
    SCARD_W_UNRESPONSIVE_CARD = 0x80100066,
    /// <summary>Power has been removed from the smart card, so that further communication is not possible.</summary>
    SCARD_W_UNPOWERED_CARD = 0x80100067,
    /// <summary>The smart card has been reset, so any shared state information is invalid.</summary>
    SCARD_W_RESET_CARD = 0x80100068,
    /// <summary>The smart card has been removed, so that further communication is not possible.</summary>
    SCARD_W_REMOVED_CARD = 0x80100069,
    /// <summary>Access was denied because of a security violation.</summary>
    SCARD_W_SECURITY_VIOLATION = 0x8010006A,
    /// <summary>The card cannot be accessed because the wrong PIN was presented.</summary>
    SCARD_W_WRONG_CHV = 0x8010006B,
    /// <summary>The card cannot be accessed because the maximum number of PIN entry attempts has been reached.</summary>
    SCARD_W_CHV_BLOCKED = 0x8010006C,
    /// <summary>The end of the smart card file has been reached.</summary>
    SCARD_W_EOF = 0x8010006D,
    /// <summary>The action was cancelled by the user.</summary>
    SCARD_W_CANCELLED_BY_USER = 0x8010006E,
    /// <summary>No PIN was presented to the smart card.</summary>
    SCARD_W_CARD_NOT_AUTHENTICATED = 0x8010006F,
    /// <summary>The requested item could not be found in the cache.</summary>
    SCARD_W_CACHE_ITEM_NOT_FOUND = 0x80100070,
    /// <summary>The requested cache item is too old and was deleted from the cache.</summary>
    SCARD_W_CACHE_ITEM_STALE = 0x80100071,
    /// <summary>The new cache item exceeds the maximum per-item size defined for the cache.</summary>
    SCARD_W_CACHE_ITEM_TOO_BIG = 0x80100072,
    /// <summary>Errors occurred accessing one or more objects - the ErrorInfo collection may have more detail</summary>
    COMADMIN_E_OBJECTERRORS = 0x80110401,
    /// <summary>One or more of the object&#39;s properties are missing or invalid</summary>
    COMADMIN_E_OBJECTINVALID = 0x80110402,
    /// <summary>The object was not found in the catalog</summary>
    COMADMIN_E_KEYMISSING = 0x80110403,
    /// <summary>The object is already registered</summary>
    COMADMIN_E_ALREADYINSTALLED = 0x80110404,
    /// <summary>Error occurred writing to the application file</summary>
    COMADMIN_E_APP_FILE_WRITEFAIL = 0x80110407,
    /// <summary>Error occurred reading the application file</summary>
    COMADMIN_E_APP_FILE_READFAIL = 0x80110408,
    /// <summary>Invalid version number in application file</summary>
    COMADMIN_E_APP_FILE_VERSION = 0x80110409,
    /// <summary>The file path is invalid</summary>
    COMADMIN_E_BADPATH = 0x8011040A,
    /// <summary>The application is already installed</summary>
    COMADMIN_E_APPLICATIONEXISTS = 0x8011040B,
    /// <summary>The role already exists</summary>
    COMADMIN_E_ROLEEXISTS = 0x8011040C,
    /// <summary>An error occurred copying the file</summary>
    COMADMIN_E_CANTCOPYFILE = 0x8011040D,
    /// <summary>One or more users are not valid</summary>
    COMADMIN_E_NOUSER = 0x8011040F,
    /// <summary>One or more users in the application file are not valid</summary>
    COMADMIN_E_INVALIDUSERIDS = 0x80110410,
    /// <summary>The component&#39;s CLSID is missing or corrupt</summary>
    COMADMIN_E_NOREGISTRYCLSID = 0x80110411,
    /// <summary>The component&#39;s progID is missing or corrupt</summary>
    COMADMIN_E_BADREGISTRYPROGID = 0x80110412,
    /// <summary>Unable to set required authentication level for update request</summary>
    COMADMIN_E_AUTHENTICATIONLEVEL = 0x80110413,
    /// <summary>The identity or password set on the application is not valid</summary>
    COMADMIN_E_USERPASSWDNOTVALID = 0x80110414,
    /// <summary>Application file CLSIDs or IIDs do not match corresponding DLLs</summary>
    COMADMIN_E_CLSIDORIIDMISMATCH = 0x80110418,
    /// <summary>Interface information is either missing or changed</summary>
    COMADMIN_E_REMOTEINTERFACE = 0x80110419,
    /// <summary>DllRegisterServer failed on component install</summary>
    COMADMIN_E_DLLREGISTERSERVER = 0x8011041A,
    /// <summary>No server file share available</summary>
    COMADMIN_E_NOSERVERSHARE = 0x8011041B,
    /// <summary>DLL could not be loaded</summary>
    COMADMIN_E_DLLLOADFAILED = 0x8011041D,
    /// <summary>The registered TypeLib ID is not valid</summary>
    COMADMIN_E_BADREGISTRYLIBID = 0x8011041E,
    /// <summary>Application install directory not found</summary>
    COMADMIN_E_APPDIRNOTFOUND = 0x8011041F,
    /// <summary>Errors occurred while in the component registrar</summary>
    COMADMIN_E_REGISTRARFAILED = 0x80110423,
    /// <summary>The file does not exist</summary>
    COMADMIN_E_COMPFILE_DOESNOTEXIST = 0x80110424,
    /// <summary>The DLL could not be loaded</summary>
    COMADMIN_E_COMPFILE_LOADDLLFAIL = 0x80110425,
    /// <summary>GetClassObject failed in the DLL</summary>
    COMADMIN_E_COMPFILE_GETCLASSOBJ = 0x80110426,
    /// <summary>The DLL does not support the components listed in the TypeLib</summary>
    COMADMIN_E_COMPFILE_CLASSNOTAVAIL = 0x80110427,
    /// <summary>The TypeLib could not be loaded</summary>
    COMADMIN_E_COMPFILE_BADTLB = 0x80110428,
    /// <summary>The file does not contain components or component information</summary>
    COMADMIN_E_COMPFILE_NOTINSTALLABLE = 0x80110429,
    /// <summary>Changes to this object and its sub-objects have been disabled</summary>
    COMADMIN_E_NOTCHANGEABLE = 0x8011042A,
    /// <summary>The delete function has been disabled for this object</summary>
    COMADMIN_E_NOTDELETEABLE = 0x8011042B,
    /// <summary>The server catalog version is not supported</summary>
    COMADMIN_E_SESSION = 0x8011042C,
    /// <summary>The component move was disallowed, because the source or destination application is either a system application or currently locked against changes</summary>
    COMADMIN_E_COMP_MOVE_LOCKED = 0x8011042D,
    /// <summary>The component move failed because the destination application no longer exists</summary>
    COMADMIN_E_COMP_MOVE_BAD_DEST = 0x8011042E,
    /// <summary>The system was unable to register the TypeLib</summary>
    COMADMIN_E_REGISTERTLB = 0x80110430,
    /// <summary>This operation cannot be performed on the system application</summary>
    COMADMIN_E_SYSTEMAPP = 0x80110433,
    /// <summary>The component registrar referenced in this file is not available</summary>
    COMADMIN_E_COMPFILE_NOREGISTRAR = 0x80110434,
    /// <summary>A component in the same DLL is already installed</summary>
    COMADMIN_E_COREQCOMPINSTALLED = 0x80110435,
    /// <summary>The service is not installed</summary>
    COMADMIN_E_SERVICENOTINSTALLED = 0x80110436,
    /// <summary>One or more property settings are either invalid or in conflict with each other</summary>
    COMADMIN_E_PROPERTYSAVEFAILED = 0x80110437,
    /// <summary>The object you are attempting to add or rename already exists</summary>
    COMADMIN_E_OBJECTEXISTS = 0x80110438,
    /// <summary>The component already exists</summary>
    COMADMIN_E_COMPONENTEXISTS = 0x80110439,
    /// <summary>The registration file is corrupt</summary>
    COMADMIN_E_REGFILE_CORRUPT = 0x8011043B,
    /// <summary>The property value is too large</summary>
    COMADMIN_E_PROPERTY_OVERFLOW = 0x8011043C,
    /// <summary>Object was not found in registry</summary>
    COMADMIN_E_NOTINREGISTRY = 0x8011043E,
    /// <summary>This object is not poolable</summary>
    COMADMIN_E_OBJECTNOTPOOLABLE = 0x8011043F,
    /// <summary>A CLSID with the same GUID as the new application ID is already installed on this machine</summary>
    COMADMIN_E_APPLID_MATCHES_CLSID = 0x80110446,
    /// <summary>A role assigned to a component, interface, or method did not exist in the application</summary>
    COMADMIN_E_ROLE_DOES_NOT_EXIST = 0x80110447,
    /// <summary>You must have components in an application in order to start the application</summary>
    COMADMIN_E_START_APP_NEEDS_COMPONENTS = 0x80110448,
    /// <summary>This operation is not enabled on this platform</summary>
    COMADMIN_E_REQUIRES_DIFFERENT_PLATFORM = 0x80110449,
    /// <summary>Application Proxy is not exportable</summary>
    COMADMIN_E_CAN_NOT_EXPORT_APP_PROXY = 0x8011044A,
    /// <summary>Failed to start application because it is either a library application or an application proxy</summary>
    COMADMIN_E_CAN_NOT_START_APP = 0x8011044B,
    /// <summary>System application is not exportable</summary>
    COMADMIN_E_CAN_NOT_EXPORT_SYS_APP = 0x8011044C,
    /// <summary>Cannot subscribe to this component (the component may have been imported)</summary>
    COMADMIN_E_CANT_SUBSCRIBE_TO_COMPONENT = 0x8011044D,
    /// <summary>An event class cannot also be a subscriber component</summary>
    COMADMIN_E_EVENTCLASS_CANT_BE_SUBSCRIBER = 0x8011044E,
    /// <summary>Library applications and application proxies are incompatible</summary>
    COMADMIN_E_LIB_APP_PROXY_INCOMPATIBLE = 0x8011044F,
    /// <summary>This function is valid for the base partition only</summary>
    COMADMIN_E_BASE_PARTITION_ONLY = 0x80110450,
    /// <summary>You cannot start an application that has been disabled</summary>
    COMADMIN_E_START_APP_DISABLED = 0x80110451,
    /// <summary>The specified partition name is already in use on this computer</summary>
    COMADMIN_E_CAT_DUPLICATE_PARTITION_NAME = 0x80110457,
    /// <summary>The specified partition name is invalid. Check that the name contains at least one visible character</summary>
    COMADMIN_E_CAT_INVALID_PARTITION_NAME = 0x80110458,
    /// <summary>The partition cannot be deleted because it is the default partition for one or more users</summary>
    COMADMIN_E_CAT_PARTITION_IN_USE = 0x80110459,
    /// <summary>The partition cannot be exported, because one or more components in the partition have the same file name</summary>
    COMADMIN_E_FILE_PARTITION_DUPLICATE_FILES = 0x8011045A,
    /// <summary>Applications that contain one or more imported components cannot be installed into a non-base partition</summary>
    COMADMIN_E_CAT_IMPORTED_COMPONENTS_NOT_ALLOWED = 0x8011045B,
    /// <summary>The application name is not unique and cannot be resolved to an application id</summary>
    COMADMIN_E_AMBIGUOUS_APPLICATION_NAME = 0x8011045C,
    /// <summary>The partition name is not unique and cannot be resolved to a partition id</summary>
    COMADMIN_E_AMBIGUOUS_PARTITION_NAME = 0x8011045D,
    /// <summary>The COM+ registry database has not been initialized</summary>
    COMADMIN_E_REGDB_NOTINITIALIZED = 0x80110472,
    /// <summary>The COM+ registry database is not open</summary>
    COMADMIN_E_REGDB_NOTOPEN = 0x80110473,
    /// <summary>The COM+ registry database detected a system error</summary>
    COMADMIN_E_REGDB_SYSTEMERR = 0x80110474,
    /// <summary>The COM+ registry database is already running</summary>
    COMADMIN_E_REGDB_ALREADYRUNNING = 0x80110475,
    /// <summary>This version of the COM+ registry database cannot be migrated</summary>
    COMADMIN_E_MIG_VERSIONNOTSUPPORTED = 0x80110480,
    /// <summary>The schema version to be migrated could not be found in the COM+ registry database</summary>
    COMADMIN_E_MIG_SCHEMANOTFOUND = 0x80110481,
    /// <summary>There was a type mismatch between binaries</summary>
    COMADMIN_E_CAT_BITNESSMISMATCH = 0x80110482,
    /// <summary>A binary of unknown or invalid type was provided</summary>
    COMADMIN_E_CAT_UNACCEPTABLEBITNESS = 0x80110483,
    /// <summary>There was a type mismatch between a binary and an application</summary>
    COMADMIN_E_CAT_WRONGAPPBITNESS = 0x80110484,
    /// <summary>The application cannot be paused or resumed</summary>
    COMADMIN_E_CAT_PAUSE_RESUME_NOT_SUPPORTED = 0x80110485,
    /// <summary>The COM+ Catalog Server threw an exception during execution</summary>
    COMADMIN_E_CAT_SERVERFAULT = 0x80110486,
    /// <summary>Only COM+ Applications marked &quot;queued&quot; can be invoked using the &quot;queue&quot; moniker</summary>
    COMQC_E_APPLICATION_NOT_QUEUED = 0x80110600,
    /// <summary>At least one interface must be marked &quot;queued&quot; in order to create a queued component instance with the &quot;queue&quot; moniker</summary>
    COMQC_E_NO_QUEUEABLE_INTERFACES = 0x80110601,
    /// <summary>MSMQ is required for the requested operation and is not installed</summary>
    COMQC_E_QUEUING_SERVICE_NOT_AVAILABLE = 0x80110602,
    /// <summary>Unable to marshal an interface that does not support IPersistStream</summary>
    COMQC_E_NO_IPERSISTSTREAM = 0x80110603,
    /// <summary>The message is improperly formatted or was damaged in transit</summary>
    COMQC_E_BAD_MESSAGE = 0x80110604,
    /// <summary>An unauthenticated message was received by an application that accepts only authenticated messages</summary>
    COMQC_E_UNAUTHENTICATED = 0x80110605,
    /// <summary>The message was requeued or moved by a user not in the &quot;QC Trusted User&quot; role</summary>
    COMQC_E_UNTRUSTED_ENQUEUER = 0x80110606,
    /// <summary>Cannot create a duplicate resource of type Distributed Transaction Coordinator</summary>
    MSDTC_E_DUPLICATE_RESOURCE = 0x80110701,
    /// <summary>One of the objects being inserted or updated does not belong to a valid parent collection</summary>
    COMADMIN_E_OBJECT_PARENT_MISSING = 0x80110808,
    /// <summary>One of the specified objects cannot be found</summary>
    COMADMIN_E_OBJECT_DOES_NOT_EXIST = 0x80110809,
    /// <summary>The specified application is not currently running</summary>
    COMADMIN_E_APP_NOT_RUNNING = 0x8011080A,
    /// <summary>The partition(s) specified are not valid.</summary>
    COMADMIN_E_INVALID_PARTITION = 0x8011080B,
    /// <summary>COM+ applications that run as NT service may not be pooled or recycled</summary>
    COMADMIN_E_SVCAPP_NOT_POOLABLE_OR_RECYCLABLE = 0x8011080D,
    /// <summary>One or more users are already assigned to a local partition set.</summary>
    COMADMIN_E_USER_IN_SET = 0x8011080E,
    /// <summary>Library applications may not be recycled.</summary>
    COMADMIN_E_CANTRECYCLELIBRARYAPPS = 0x8011080F,
    /// <summary>Applications running as NT services may not be recycled.</summary>
    COMADMIN_E_CANTRECYCLESERVICEAPPS = 0x80110811,
    /// <summary>The process has already been recycled.</summary>
    COMADMIN_E_PROCESSALREADYRECYCLED = 0x80110812,
    /// <summary>A paused process may not be recycled.</summary>
    COMADMIN_E_PAUSEDPROCESSMAYNOTBERECYCLED = 0x80110813,
    /// <summary>Library applications may not be NT services.</summary>
    COMADMIN_E_CANTMAKEINPROCSERVICE = 0x80110814,
    /// <summary>The ProgID provided to the copy operation is invalid. The ProgID is in use by another registered CLSID.</summary>
    COMADMIN_E_PROGIDINUSEBYCLSID = 0x80110815,
    /// <summary>The partition specified as default is not a member of the partition set.</summary>
    COMADMIN_E_DEFAULT_PARTITION_NOT_IN_SET = 0x80110816,
    /// <summary>A recycled process may not be paused.</summary>
    COMADMIN_E_RECYCLEDPROCESSMAYNOTBEPAUSED = 0x80110817,
    /// <summary>Access to the specified partition is denied.</summary>
    COMADMIN_E_PARTITION_ACCESSDENIED = 0x80110818,
    /// <summary>Only Application Files (*.MSI files) can be installed into partitions.</summary>
    COMADMIN_E_PARTITION_MSI_ONLY = 0x80110819,
    /// <summary>Applications containing one or more legacy components may not be exported to 1.0 format.</summary>
    COMADMIN_E_LEGACYCOMPS_NOT_ALLOWED_IN_1_0_FORMAT = 0x8011081A,
    /// <summary>Legacy components may not exist in non-base partitions.</summary>
    COMADMIN_E_LEGACYCOMPS_NOT_ALLOWED_IN_NONBASE_PARTITIONS = 0x8011081B,
    /// <summary>A component cannot be moved (or copied) from the System Application, an application proxy or a non-changeable application</summary>
    COMADMIN_E_COMP_MOVE_SOURCE = 0x8011081C,
    /// <summary>A component cannot be moved (or copied) to the System Application, an application proxy or a non-changeable application</summary>
    COMADMIN_E_COMP_MOVE_DEST = 0x8011081D,
    /// <summary>A private component cannot be moved (or copied) to a library application or to the base partition</summary>
    COMADMIN_E_COMP_MOVE_PRIVATE = 0x8011081E,
    /// <summary>The Base Application Partition exists in all partition sets and cannot be removed.</summary>
    COMADMIN_E_BASEPARTITION_REQUIRED_IN_SET = 0x8011081F,
    /// <summary>Alas, Event Class components cannot be aliased.</summary>
    COMADMIN_E_CANNOT_ALIAS_EVENTCLASS = 0x80110820,
    /// <summary>Access is denied because the component is private.</summary>
    COMADMIN_E_PRIVATE_ACCESSDENIED = 0x80110821,
    /// <summary>The specified SAFER level is invalid.</summary>
    COMADMIN_E_SAFERINVALID = 0x80110822,
    /// <summary>The specified user cannot write to the system registry</summary>
    COMADMIN_E_REGISTRY_ACCESSDENIED = 0x80110823,
    /// <summary>COM+ partitions are currently disabled.</summary>
    COMADMIN_E_PARTITIONS_DISABLED = 0x80110824,
    /// <summary>The IO was completed by a filter.</summary>
    ERROR_FLT_IO_COMPLETE = 0x001F0001,
    /// <summary>A handler was not defined by the filter for this operation.</summary>
    ERROR_FLT_NO_HANDLER_DEFINED = 0x801F0001,
    /// <summary>A context is already defined for this object.</summary>
    ERROR_FLT_CONTEXT_ALREADY_DEFINED = 0x801F0002,
    /// <summary>Asynchronous requests are not valid for this operation.</summary>
    ERROR_FLT_INVALID_ASYNCHRONOUS_REQUEST = 0x801F0003,
    /// <summary>Disallow the Fast IO path for this operation.</summary>
    ERROR_FLT_DISALLOW_FAST_IO = 0x801F0004,
    /// <summary>An invalid name request was made. The name requested cannot be retrieved at this time.</summary>
    ERROR_FLT_INVALID_NAME_REQUEST = 0x801F0005,
    /// <summary>Posting this operation to a worker thread for further processing is not safe at this time because it could lead to a system deadlock.</summary>
    ERROR_FLT_NOT_SAFE_TO_POST_OPERATION = 0x801F0006,
    /// <summary>The Filter Manager was not initialized when a filter tried to register. Make sure that the Filter Manager is getting loaded as a driver.</summary>
    ERROR_FLT_NOT_INITIALIZED = 0x801F0007,
    /// <summary>The filter is not ready for attachment to volumes because it has not finished initializing (FltStartFiltering has not been called).</summary>
    ERROR_FLT_FILTER_NOT_READY = 0x801F0008,
    /// <summary>The filter must cleanup any operation specific context at this time because it is being removed from the system before the operation is completed by the lower drivers.</summary>
    ERROR_FLT_POST_OPERATION_CLEANUP = 0x801F0009,
    /// <summary>The Filter Manager had an internal error from which it cannot recover, therefore the operation has been failed. This is usually the result of a filter returning an invalid value from a pre-operation callback.</summary>
    ERROR_FLT_INTERNAL_ERROR = 0x801F000A,
    /// <summary>The object specified for this action is in the process of being deleted, therefore the action requested cannot be completed at this time.</summary>
    ERROR_FLT_DELETING_OBJECT = 0x801F000B,
    /// <summary>Non-paged pool must be used for this type of context.</summary>
    ERROR_FLT_MUST_BE_NONPAGED_POOL = 0x801F000C,
    /// <summary>A duplicate handler definition has been provided for an operation.</summary>
    ERROR_FLT_DUPLICATE_ENTRY = 0x801F000D,
    /// <summary>The callback data queue has been disabled.</summary>
    ERROR_FLT_CBDQ_DISABLED = 0x801F000E,
    /// <summary>Do not attach the filter to the volume at this time.</summary>
    ERROR_FLT_DO_NOT_ATTACH = 0x801F000F,
    /// <summary>Do not detach the filter from the volume at this time.</summary>
    ERROR_FLT_DO_NOT_DETACH = 0x801F0010,
    /// <summary>An instance already exists at this altitude on the volume specified.</summary>
    ERROR_FLT_INSTANCE_ALTITUDE_COLLISION = 0x801F0011,
    /// <summary>An instance already exists with this name on the volume specified.</summary>
    ERROR_FLT_INSTANCE_NAME_COLLISION = 0x801F0012,
    /// <summary>The system could not find the filter specified.</summary>
    ERROR_FLT_FILTER_NOT_FOUND = 0x801F0013,
    /// <summary>The system could not find the volume specified.</summary>
    ERROR_FLT_VOLUME_NOT_FOUND = 0x801F0014,
    /// <summary>The system could not find the instance specified.</summary>
    ERROR_FLT_INSTANCE_NOT_FOUND = 0x801F0015,
    /// <summary>No registered context allocation definition was found for the given request.</summary>
    ERROR_FLT_CONTEXT_ALLOCATION_NOT_FOUND = 0x801F0016,
    /// <summary>An invalid parameter was specified during context registration.</summary>
    ERROR_FLT_INVALID_CONTEXT_REGISTRATION = 0x801F0017,
    /// <summary>The name requested was not found in Filter Manager&#39;s name cache and could not be retrieved from the file system.</summary>
    ERROR_FLT_NAME_CACHE_MISS = 0x801F0018,
    /// <summary>The requested device object does not exist for the given volume.</summary>
    ERROR_FLT_NO_DEVICE_OBJECT = 0x801F0019,
    /// <summary>The specified volume is already mounted.</summary>
    ERROR_FLT_VOLUME_ALREADY_MOUNTED = 0x801F001A,
    /// <summary>The specified Transaction Context is already enlisted in a transaction</summary>
    ERROR_FLT_ALREADY_ENLISTED = 0x801F001B,
    /// <summary>The specifiec context is already attached to another object</summary>
    ERROR_FLT_CONTEXT_ALREADY_LINKED = 0x801F001C,
    /// <summary>No waiter is present for the filter&#39;s reply to this message.</summary>
    ERROR_FLT_NO_WAITER_FOR_REPLY = 0x801F0020,
    /// <summary>{Display Driver Stopped Responding} The %hs display driver has stopped working normally. Save your work and reboot the system to restore full display functionality. The next time you reboot the machine a dialog will be displayed giving you a chance to report this failure to Microsoft.</summary>
    ERROR_HUNG_DISPLAY_DRIVER_THREAD = 0x80260001,
    /// <summary>{Desktop composition is disabled} The operation could not be completed because desktop composition is disabled.</summary>
    DWM_E_COMPOSITIONDISABLED = 0x80263001,
    /// <summary>{Some desktop composition APIs are not supported while remoting} The operation is not supported while running in a remote session.</summary>
    DWM_E_REMOTING_NOT_SUPPORTED = 0x80263002,
    /// <summary>{No DWM redirection surface is available} The DWM was unable to provide a redireciton surface to complete the DirectX present.</summary>
    DWM_E_NO_REDIRECTION_SURFACE_AVAILABLE = 0x80263003,
    /// <summary>{DWM is not queuing presents for the specified window} The window specified is not currently using queued presents.</summary>
    DWM_E_NOT_QUEUING_PRESENTS = 0x80263004,
    /// <summary>{The adapter specified by the LUID is not found} DWM can not find the adapter specified by the LUID.</summary>
    DWM_E_ADAPTER_NOT_FOUND = 0x80263005,
    /// <summary>{GDI redirection surface was returned} GDI redirection surface of the top level window was returned.</summary>
    DWM_S_GDI_REDIRECTION_SURFACE = 0x00263005,
    /// <summary>Monitor descriptor could not be obtained.</summary>
    ERROR_MONITOR_NO_DESCRIPTOR = 0x00261001,
    /// <summary>Format of the obtained monitor descriptor is not supported by this release.</summary>
    ERROR_MONITOR_UNKNOWN_DESCRIPTOR_FORMAT = 0x00261002,
    /// <summary>Checksum of the obtained monitor descriptor is invalid.</summary>
    ERROR_MONITOR_INVALID_DESCRIPTOR_CHECKSUM = 0xC0261003,
    /// <summary>Monitor descriptor contains an invalid standard timing block.</summary>
    ERROR_MONITOR_INVALID_STANDARD_TIMING_BLOCK = 0xC0261004,
    /// <summary>WMI data block registration failed for one of the MSMonitorClass WMI subclasses.</summary>
    ERROR_MONITOR_WMI_DATABLOCK_REGISTRATION_FAILED = 0xC0261005,
    /// <summary>Provided monitor descriptor block is either corrupted or does not contain monitor&#39;s detailed serial number.</summary>
    ERROR_MONITOR_INVALID_SERIAL_NUMBER_MONDSC_BLOCK = 0xC0261006,
    /// <summary>Provided monitor descriptor block is either corrupted or does not contain monitor&#39;s user friendly name.</summary>
    ERROR_MONITOR_INVALID_USER_FRIENDLY_MONDSC_BLOCK = 0xC0261007,
    /// <summary>There is no monitor descriptor data at the specified (offset, size) region.</summary>
    ERROR_MONITOR_NO_MORE_DESCRIPTOR_DATA = 0xC0261008,
    /// <summary>Monitor descriptor contains an invalid detailed timing block.</summary>
    ERROR_MONITOR_INVALID_DETAILED_TIMING_BLOCK = 0xC0261009,
    /// <summary>Monitor descriptor contains invalid manufacture date.</summary>
    ERROR_MONITOR_INVALID_MANUFACTURE_DATE = 0xC026100A,
    /// <summary>Exclusive mode ownership is needed to create unmanaged primary allocation.</summary>
    ERROR_GRAPHICS_NOT_EXCLUSIVE_MODE_OWNER = 0xC0262000,
    /// <summary>The driver needs more DMA buffer space in order to complete the requested operation.</summary>
    ERROR_GRAPHICS_INSUFFICIENT_DMA_BUFFER = 0xC0262001,
    /// <summary>Specified display adapter handle is invalid.</summary>
    ERROR_GRAPHICS_INVALID_DISPLAY_ADAPTER = 0xC0262002,
    /// <summary>Specified display adapter and all of its state has been reset.</summary>
    ERROR_GRAPHICS_ADAPTER_WAS_RESET = 0xC0262003,
    /// <summary>The driver stack doesn&#39;t match the expected driver model.</summary>
    ERROR_GRAPHICS_INVALID_DRIVER_MODEL = 0xC0262004,
    /// <summary>Present happened but ended up into the changed desktop mode</summary>
    ERROR_GRAPHICS_PRESENT_MODE_CHANGED = 0xC0262005,
    /// <summary>Nothing to present due to desktop occlusion</summary>
    ERROR_GRAPHICS_PRESENT_OCCLUDED = 0xC0262006,
    /// <summary>Not able to present due to denial of desktop access</summary>
    ERROR_GRAPHICS_PRESENT_DENIED = 0xC0262007,
    /// <summary>Not able to present with color convertion</summary>
    ERROR_GRAPHICS_CANNOTCOLORCONVERT = 0xC0262008,
    /// <summary>The kernel driver detected a version mismatch between it and the user mode driver.</summary>
    ERROR_GRAPHICS_DRIVER_MISMATCH = 0xC0262009,
    /// <summary>Specified buffer is not big enough to contain entire requested dataset. Partial data populated up to the size of the buffer. Caller needs to provide buffer of size as specified in the partially populated buffer&#39;s content (interface specific).</summary>
    ERROR_GRAPHICS_PARTIAL_DATA_POPULATED = 0x4026200A,
    /// <summary>Present redirection is disabled (desktop windowing management subsystem is off).</summary>
    ERROR_GRAPHICS_PRESENT_REDIRECTION_DISABLED = 0xC026200B,
    /// <summary>Previous exclusive VidPn source owner has released its ownership</summary>
    ERROR_GRAPHICS_PRESENT_UNOCCLUDED = 0xC026200C,
    /// <summary>Not enough video memory available to complete the operation.</summary>
    ERROR_GRAPHICS_NO_VIDEO_MEMORY = 0xC0262100,
    /// <summary>Couldn&#39;t probe and lock the underlying memory of an allocation.</summary>
    ERROR_GRAPHICS_CANT_LOCK_MEMORY = 0xC0262101,
    /// <summary>The allocation is currently busy.</summary>
    ERROR_GRAPHICS_ALLOCATION_BUSY = 0xC0262102,
    /// <summary>An object being referenced has reach the maximum reference count already and can&#39;t be reference further.</summary>
    ERROR_GRAPHICS_TOO_MANY_REFERENCES = 0xC0262103,
    /// <summary>A problem couldn&#39;t be solved due to some currently existing condition. The problem should be tried again later.</summary>
    ERROR_GRAPHICS_TRY_AGAIN_LATER = 0xC0262104,
    /// <summary>A problem couldn&#39;t be solved due to some currently existing condition. The problem should be tried again immediately.</summary>
    ERROR_GRAPHICS_TRY_AGAIN_NOW = 0xC0262105,
    /// <summary>The allocation is invalid.</summary>
    ERROR_GRAPHICS_ALLOCATION_INVALID = 0xC0262106,
    /// <summary>No more unswizzling aperture are currently available.</summary>
    ERROR_GRAPHICS_UNSWIZZLING_APERTURE_UNAVAILABLE = 0xC0262107,
    /// <summary>The current allocation can&#39;t be unswizzled by an aperture.</summary>
    ERROR_GRAPHICS_UNSWIZZLING_APERTURE_UNSUPPORTED = 0xC0262108,
    /// <summary>The request failed because a pinned allocation can&#39;t be evicted.</summary>
    ERROR_GRAPHICS_CANT_EVICT_PINNED_ALLOCATION = 0xC0262109,
    /// <summary>The allocation can&#39;t be used from it&#39;s current segment location for the specified operation.</summary>
    ERROR_GRAPHICS_INVALID_ALLOCATION_USAGE = 0xC0262110,
    /// <summary>A locked allocation can&#39;t be used in the current command buffer.</summary>
    ERROR_GRAPHICS_CANT_RENDER_LOCKED_ALLOCATION = 0xC0262111,
    /// <summary>The allocation being referenced has been closed permanently.</summary>
    ERROR_GRAPHICS_ALLOCATION_CLOSED = 0xC0262112,
    /// <summary>An invalid allocation instance is being referenced.</summary>
    ERROR_GRAPHICS_INVALID_ALLOCATION_INSTANCE = 0xC0262113,
    /// <summary>An invalid allocation handle is being referenced.</summary>
    ERROR_GRAPHICS_INVALID_ALLOCATION_HANDLE = 0xC0262114,
    /// <summary>The allocation being referenced doesn&#39;t belong to the current device.</summary>
    ERROR_GRAPHICS_WRONG_ALLOCATION_DEVICE = 0xC0262115,
    /// <summary>The specified allocation lost its content.</summary>
    ERROR_GRAPHICS_ALLOCATION_CONTENT_LOST = 0xC0262116,
    /// <summary>GPU exception is detected on the given device. The device is not able to be scheduled.</summary>
    ERROR_GRAPHICS_GPU_EXCEPTION_ON_DEVICE = 0xC0262200,
    /// <summary>Specified VidPN topology is invalid.</summary>
    ERROR_GRAPHICS_INVALID_VIDPN_TOPOLOGY = 0xC0262300,
    /// <summary>Specified VidPN topology is valid but is not supported by this model of the display adapter.</summary>
    ERROR_GRAPHICS_VIDPN_TOPOLOGY_NOT_SUPPORTED = 0xC0262301,
    /// <summary>Specified VidPN topology is valid but is not supported by the display adapter at this time, due to current allocation of its resources.</summary>
    ERROR_GRAPHICS_VIDPN_TOPOLOGY_CURRENTLY_NOT_SUPPORTED = 0xC0262302,
    /// <summary>Specified VidPN handle is invalid.</summary>
    ERROR_GRAPHICS_INVALID_VIDPN = 0xC0262303,
    /// <summary>Specified video present source is invalid.</summary>
    ERROR_GRAPHICS_INVALID_VIDEO_PRESENT_SOURCE = 0xC0262304,
    /// <summary>Specified video present target is invalid.</summary>
    ERROR_GRAPHICS_INVALID_VIDEO_PRESENT_TARGET = 0xC0262305,
    /// <summary>Specified VidPN modality is not supported (e.g. at least two of the pinned modes are not cofunctional).</summary>
    ERROR_GRAPHICS_VIDPN_MODALITY_NOT_SUPPORTED = 0xC0262306,
    /// <summary>No mode is pinned on the specified VidPN source/target.</summary>
    ERROR_GRAPHICS_MODE_NOT_PINNED = 0x00262307,
    /// <summary>Specified VidPN source mode set is invalid.</summary>
    ERROR_GRAPHICS_INVALID_VIDPN_SOURCEMODESET = 0xC0262308,
    /// <summary>Specified VidPN target mode set is invalid.</summary>
    ERROR_GRAPHICS_INVALID_VIDPN_TARGETMODESET = 0xC0262309,
    /// <summary>Specified video signal frequency is invalid.</summary>
    ERROR_GRAPHICS_INVALID_FREQUENCY = 0xC026230A,
    /// <summary>Specified video signal active region is invalid.</summary>
    ERROR_GRAPHICS_INVALID_ACTIVE_REGION = 0xC026230B,
    /// <summary>Specified video signal total region is invalid.</summary>
    ERROR_GRAPHICS_INVALID_TOTAL_REGION = 0xC026230C,
    /// <summary>Specified video present source mode is invalid.</summary>
    ERROR_GRAPHICS_INVALID_VIDEO_PRESENT_SOURCE_MODE = 0xC0262310,
    /// <summary>Specified video present target mode is invalid.</summary>
    ERROR_GRAPHICS_INVALID_VIDEO_PRESENT_TARGET_MODE = 0xC0262311,
    /// <summary>Pinned mode must remain in the set on VidPN&#39;s cofunctional modality enumeration.</summary>
    ERROR_GRAPHICS_PINNED_MODE_MUST_REMAIN_IN_SET = 0xC0262312,
    /// <summary>Specified video present path is already in VidPN&#39;s topology.</summary>
    ERROR_GRAPHICS_PATH_ALREADY_IN_TOPOLOGY = 0xC0262313,
    /// <summary>Specified mode is already in the mode set.</summary>
    ERROR_GRAPHICS_MODE_ALREADY_IN_MODESET = 0xC0262314,
    /// <summary>Specified video present source set is invalid.</summary>
    ERROR_GRAPHICS_INVALID_VIDEOPRESENTSOURCESET = 0xC0262315,
    /// <summary>Specified video present target set is invalid.</summary>
    ERROR_GRAPHICS_INVALID_VIDEOPRESENTTARGETSET = 0xC0262316,
    /// <summary>Specified video present source is already in the video present source set.</summary>
    ERROR_GRAPHICS_SOURCE_ALREADY_IN_SET = 0xC0262317,
    /// <summary>Specified video present target is already in the video present target set.</summary>
    ERROR_GRAPHICS_TARGET_ALREADY_IN_SET = 0xC0262318,
    /// <summary>Specified VidPN present path is invalid.</summary>
    ERROR_GRAPHICS_INVALID_VIDPN_PRESENT_PATH = 0xC0262319,
    /// <summary>Miniport has no recommendation for augmentation of the specified VidPN&#39;s topology.</summary>
    ERROR_GRAPHICS_NO_RECOMMENDED_VIDPN_TOPOLOGY = 0xC026231A,
    /// <summary>Specified monitor frequency range set is invalid.</summary>
    ERROR_GRAPHICS_INVALID_MONITOR_FREQUENCYRANGESET = 0xC026231B,
    /// <summary>Specified monitor frequency range is invalid.</summary>
    ERROR_GRAPHICS_INVALID_MONITOR_FREQUENCYRANGE = 0xC026231C,
    /// <summary>Specified frequency range is not in the specified monitor frequency range set.</summary>
    ERROR_GRAPHICS_FREQUENCYRANGE_NOT_IN_SET = 0xC026231D,
    /// <summary>Specified mode set does not specify preference for one of its modes.</summary>
    ERROR_GRAPHICS_NO_PREFERRED_MODE = 0x0026231E,
    /// <summary>Specified frequency range is already in the specified monitor frequency range set.</summary>
    ERROR_GRAPHICS_FREQUENCYRANGE_ALREADY_IN_SET = 0xC026231F,
    /// <summary>Specified mode set is stale. Please reacquire the new mode set.</summary>
    ERROR_GRAPHICS_STALE_MODESET = 0xC0262320,
    /// <summary>Specified monitor source mode set is invalid.</summary>
    ERROR_GRAPHICS_INVALID_MONITOR_SOURCEMODESET = 0xC0262321,
    /// <summary>Specified monitor source mode is invalid.</summary>
    ERROR_GRAPHICS_INVALID_MONITOR_SOURCE_MODE = 0xC0262322,
    /// <summary>Miniport does not have any recommendation regarding the request to provide a functional VidPN given the current display adapter configuration.</summary>
    ERROR_GRAPHICS_NO_RECOMMENDED_FUNCTIONAL_VIDPN = 0xC0262323,
    /// <summary>ID of the specified mode is already used by another mode in the set.</summary>
    ERROR_GRAPHICS_MODE_ID_MUST_BE_UNIQUE = 0xC0262324,
    /// <summary>System failed to determine a mode that is supported by both the display adapter and the monitor connected to it.</summary>
    ERROR_GRAPHICS_EMPTY_ADAPTER_MONITOR_MODE_SUPPORT_INTERSECTION = 0xC0262325,
    /// <summary>Number of video present targets must be greater than or equal to the number of video present sources.</summary>
    ERROR_GRAPHICS_VIDEO_PRESENT_TARGETS_LESS_THAN_SOURCES = 0xC0262326,
    /// <summary>Specified present path is not in VidPN&#39;s topology.</summary>
    ERROR_GRAPHICS_PATH_NOT_IN_TOPOLOGY = 0xC0262327,
    /// <summary>Display adapter must have at least one video present source.</summary>
    ERROR_GRAPHICS_ADAPTER_MUST_HAVE_AT_LEAST_ONE_SOURCE = 0xC0262328,
    /// <summary>Display adapter must have at least one video present target.</summary>
    ERROR_GRAPHICS_ADAPTER_MUST_HAVE_AT_LEAST_ONE_TARGET = 0xC0262329,
    /// <summary>Specified monitor descriptor set is invalid.</summary>
    ERROR_GRAPHICS_INVALID_MONITORDESCRIPTORSET = 0xC026232A,
    /// <summary>Specified monitor descriptor is invalid.</summary>
    ERROR_GRAPHICS_INVALID_MONITORDESCRIPTOR = 0xC026232B,
    /// <summary>Specified descriptor is not in the specified monitor descriptor set.</summary>
    ERROR_GRAPHICS_MONITORDESCRIPTOR_NOT_IN_SET = 0xC026232C,
    /// <summary>Specified descriptor is already in the specified monitor descriptor set.</summary>
    ERROR_GRAPHICS_MONITORDESCRIPTOR_ALREADY_IN_SET = 0xC026232D,
    /// <summary>ID of the specified monitor descriptor is already used by another descriptor in the set.</summary>
    ERROR_GRAPHICS_MONITORDESCRIPTOR_ID_MUST_BE_UNIQUE = 0xC026232E,
    /// <summary>Specified video present target subset type is invalid.</summary>
    ERROR_GRAPHICS_INVALID_VIDPN_TARGET_SUBSET_TYPE = 0xC026232F,
    /// <summary>Two or more of the specified resources are not related to each other, as defined by the interface semantics.</summary>
    ERROR_GRAPHICS_RESOURCES_NOT_RELATED = 0xC0262330,
    /// <summary>ID of the specified video present source is already used by another source in the set.</summary>
    ERROR_GRAPHICS_SOURCE_ID_MUST_BE_UNIQUE = 0xC0262331,
    /// <summary>ID of the specified video present target is already used by another target in the set.</summary>
    ERROR_GRAPHICS_TARGET_ID_MUST_BE_UNIQUE = 0xC0262332,
    /// <summary>Specified VidPN source cannot be used because there is no available VidPN target to connect it to.</summary>
    ERROR_GRAPHICS_NO_AVAILABLE_VIDPN_TARGET = 0xC0262333,
    /// <summary>Newly arrived monitor could not be associated with a display adapter.</summary>
    ERROR_GRAPHICS_MONITOR_COULD_NOT_BE_ASSOCIATED_WITH_ADAPTER = 0xC0262334,
    /// <summary>Display adapter in question does not have an associated VidPN manager.</summary>
    ERROR_GRAPHICS_NO_VIDPNMGR = 0xC0262335,
    /// <summary>VidPN manager of the display adapter in question does not have an active VidPN.</summary>
    ERROR_GRAPHICS_NO_ACTIVE_VIDPN = 0xC0262336,
    /// <summary>Specified VidPN topology is stale. Please reacquire the new topology.</summary>
    ERROR_GRAPHICS_STALE_VIDPN_TOPOLOGY = 0xC0262337,
    /// <summary>There is no monitor connected on the specified video present target.</summary>
    ERROR_GRAPHICS_MONITOR_NOT_CONNECTED = 0xC0262338,
    /// <summary>Specified source is not part of the specified VidPN&#39;s topology.</summary>
    ERROR_GRAPHICS_SOURCE_NOT_IN_TOPOLOGY = 0xC0262339,
    /// <summary>Specified primary surface size is invalid.</summary>
    ERROR_GRAPHICS_INVALID_PRIMARYSURFACE_SIZE = 0xC026233A,
    /// <summary>Specified visible region size is invalid.</summary>
    ERROR_GRAPHICS_INVALID_VISIBLEREGION_SIZE = 0xC026233B,
    /// <summary>Specified stride is invalid.</summary>
    ERROR_GRAPHICS_INVALID_STRIDE = 0xC026233C,
    /// <summary>Specified pixel format is invalid.</summary>
    ERROR_GRAPHICS_INVALID_PIXELFORMAT = 0xC026233D,
    /// <summary>Specified color basis is invalid.</summary>
    ERROR_GRAPHICS_INVALID_COLORBASIS = 0xC026233E,
    /// <summary>Specified pixel value access mode is invalid.</summary>
    ERROR_GRAPHICS_INVALID_PIXELVALUEACCESSMODE = 0xC026233F,
    /// <summary>Specified target is not part of the specified VidPN&#39;s topology.</summary>
    ERROR_GRAPHICS_TARGET_NOT_IN_TOPOLOGY = 0xC0262340,
    /// <summary>Failed to acquire display mode management interface.</summary>
    ERROR_GRAPHICS_NO_DISPLAY_MODE_MANAGEMENT_SUPPORT = 0xC0262341,
    /// <summary>Specified VidPN source is already owned by a DMM client and cannot be used until that client releases it.</summary>
    ERROR_GRAPHICS_VIDPN_SOURCE_IN_USE = 0xC0262342,
    /// <summary>Specified VidPN is active and cannot be accessed.</summary>
    ERROR_GRAPHICS_CANT_ACCESS_ACTIVE_VIDPN = 0xC0262343,
    /// <summary>Specified VidPN present path importance ordinal is invalid.</summary>
    ERROR_GRAPHICS_INVALID_PATH_IMPORTANCE_ORDINAL = 0xC0262344,
    /// <summary>Specified VidPN present path content geometry transformation is invalid.</summary>
    ERROR_GRAPHICS_INVALID_PATH_CONTENT_GEOMETRY_TRANSFORMATION = 0xC0262345,
    /// <summary>Specified content geometry transformation is not supported on the respective VidPN present path.</summary>
    ERROR_GRAPHICS_PATH_CONTENT_GEOMETRY_TRANSFORMATION_NOT_SUPPORTED = 0xC0262346,
    /// <summary>Specified gamma ramp is invalid.</summary>
    ERROR_GRAPHICS_INVALID_GAMMA_RAMP = 0xC0262347,
    /// <summary>Specified gamma ramp is not supported on the respective VidPN present path.</summary>
    ERROR_GRAPHICS_GAMMA_RAMP_NOT_SUPPORTED = 0xC0262348,
    /// <summary>Multi-sampling is not supported on the respective VidPN present path.</summary>
    ERROR_GRAPHICS_MULTISAMPLING_NOT_SUPPORTED = 0xC0262349,
    /// <summary>Specified mode is not in the specified mode set.</summary>
    ERROR_GRAPHICS_MODE_NOT_IN_MODESET = 0xC026234A,
    /// <summary>Specified data set (e.g. mode set, frequency range set, descriptor set, topology, etc.) is empty.</summary>
    ERROR_GRAPHICS_DATASET_IS_EMPTY = 0x0026234B,
    /// <summary>Specified data set (e.g. mode set, frequency range set, descriptor set, topology, etc.) does not contain any more elements.</summary>
    ERROR_GRAPHICS_NO_MORE_ELEMENTS_IN_DATASET = 0x0026234C,
    /// <summary>Specified VidPN topology recommendation reason is invalid.</summary>
    ERROR_GRAPHICS_INVALID_VIDPN_TOPOLOGY_RECOMMENDATION_REASON = 0xC026234D,
    /// <summary>Specified VidPN present path content type is invalid.</summary>
    ERROR_GRAPHICS_INVALID_PATH_CONTENT_TYPE = 0xC026234E,
    /// <summary>Specified VidPN present path copy protection type is invalid.</summary>
    ERROR_GRAPHICS_INVALID_COPYPROTECTION_TYPE = 0xC026234F,
    /// <summary>No more than one unassigned mode set can exist at any given time for a given VidPN source/target.</summary>
    ERROR_GRAPHICS_UNASSIGNED_MODESET_ALREADY_EXISTS = 0xC0262350,
    /// <summary>Specified content transformation is not pinned on the specified VidPN present path.</summary>
    ERROR_GRAPHICS_PATH_CONTENT_GEOMETRY_TRANSFORMATION_NOT_PINNED = 0x00262351,
    /// <summary>Specified scanline ordering type is invalid.</summary>
    ERROR_GRAPHICS_INVALID_SCANLINE_ORDERING = 0xC0262352,
    /// <summary>Topology changes are not allowed for the specified VidPN.</summary>
    ERROR_GRAPHICS_TOPOLOGY_CHANGES_NOT_ALLOWED = 0xC0262353,
    /// <summary>All available importance ordinals are already used in specified topology.</summary>
    ERROR_GRAPHICS_NO_AVAILABLE_IMPORTANCE_ORDINALS = 0xC0262354,
    /// <summary>Specified primary surface has a different private format attribute than the current primary surface</summary>
    ERROR_GRAPHICS_INCOMPATIBLE_PRIVATE_FORMAT = 0xC0262355,
    /// <summary>Specified mode pruning algorithm is invalid</summary>
    ERROR_GRAPHICS_INVALID_MODE_PRUNING_ALGORITHM = 0xC0262356,
    /// <summary>Specified monitor capability origin is invalid.</summary>
    ERROR_GRAPHICS_INVALID_MONITOR_CAPABILITY_ORIGIN = 0xC0262357,
    /// <summary>Specified monitor frequency range constraint is invalid.</summary>
    ERROR_GRAPHICS_INVALID_MONITOR_FREQUENCYRANGE_CONSTRAINT = 0xC0262358,
    /// <summary>Maximum supported number of present paths has been reached.</summary>
    ERROR_GRAPHICS_MAX_NUM_PATHS_REACHED = 0xC0262359,
    /// <summary>Miniport requested that augmentation be cancelled for the specified source of the specified VidPN&#39;s topology.</summary>
    ERROR_GRAPHICS_CANCEL_VIDPN_TOPOLOGY_AUGMENTATION = 0xC026235A,
    /// <summary>Specified client type was not recognized.</summary>
    ERROR_GRAPHICS_INVALID_CLIENT_TYPE = 0xC026235B,
    /// <summary>Client VidPN is not set on this adapter (e.g. no user mode initiated mode changes took place on this adapter yet).</summary>
    ERROR_GRAPHICS_CLIENTVIDPN_NOT_SET = 0xC026235C,
    /// <summary>Specified display adapter child device already has an external device connected to it.</summary>
    ERROR_GRAPHICS_SPECIFIED_CHILD_ALREADY_CONNECTED = 0xC0262400,
    /// <summary>Specified display adapter child device does not support descriptor exposure.</summary>
    ERROR_GRAPHICS_CHILD_DESCRIPTOR_NOT_SUPPORTED = 0xC0262401,
    /// <summary>Child device presence was not reliably detected.</summary>
    ERROR_GRAPHICS_UNKNOWN_CHILD_STATUS = 0x4026242F,
    /// <summary>The display adapter is not linked to any other adapters.</summary>
    ERROR_GRAPHICS_NOT_A_LINKED_ADAPTER = 0xC0262430,
    /// <summary>Lead adapter in a linked configuration was not enumerated yet.</summary>
    ERROR_GRAPHICS_LEADLINK_NOT_ENUMERATED = 0xC0262431,
    /// <summary>Some chain adapters in a linked configuration were not enumerated yet.</summary>
    ERROR_GRAPHICS_CHAINLINKS_NOT_ENUMERATED = 0xC0262432,
    /// <summary>The chain of linked adapters is not ready to start because of an unknown failure.</summary>
    ERROR_GRAPHICS_ADAPTER_CHAIN_NOT_READY = 0xC0262433,
    /// <summary>An attempt was made to start a lead link display adapter when the chain links were not started yet.</summary>
    ERROR_GRAPHICS_CHAINLINKS_NOT_STARTED = 0xC0262434,
    /// <summary>An attempt was made to power up a lead link display adapter when the chain links were powered down.</summary>
    ERROR_GRAPHICS_CHAINLINKS_NOT_POWERED_ON = 0xC0262435,
    /// <summary>The adapter link was found to be in an inconsistent state. Not all adapters are in an expected PNP/Power state.</summary>
    ERROR_GRAPHICS_INCONSISTENT_DEVICE_LINK_STATE = 0xC0262436,
    /// <summary>Starting the leadlink adapter has been deferred temporarily.</summary>
    ERROR_GRAPHICS_LEADLINK_START_DEFERRED = 0x40262437,
    /// <summary>The driver trying to start is not the same as the driver for the POSTed display adapter.</summary>
    ERROR_GRAPHICS_NOT_POST_DEVICE_DRIVER = 0xC0262438,
    /// <summary>The display adapter is being polled for children too frequently at the same polling level.</summary>
    ERROR_GRAPHICS_POLLING_TOO_FREQUENTLY = 0x40262439,
    /// <summary>Starting the adapter has been deferred temporarily.</summary>
    ERROR_GRAPHICS_START_DEFERRED = 0x4026243A,
    /// <summary>An operation is being attempted that requires the display adapter to be in a quiescent state.</summary>
    ERROR_GRAPHICS_ADAPTER_ACCESS_NOT_EXCLUDED = 0xC026243B,
    /// <summary>The driver does not support OPM.</summary>
    ERROR_GRAPHICS_OPM_NOT_SUPPORTED = 0xC0262500,
    /// <summary>The driver does not support COPP.</summary>
    ERROR_GRAPHICS_COPP_NOT_SUPPORTED = 0xC0262501,
    /// <summary>The driver does not support UAB.</summary>
    ERROR_GRAPHICS_UAB_NOT_SUPPORTED = 0xC0262502,
    /// <summary>The specified encrypted parameters are invalid.</summary>
    ERROR_GRAPHICS_OPM_INVALID_ENCRYPTED_PARAMETERS = 0xC0262503,
    /// <summary>The GDI display device passed to this function does not have any active video outputs.</summary>
    ERROR_GRAPHICS_OPM_NO_VIDEO_OUTPUTS_EXIST = 0xC0262505,
    /// <summary>An internal error caused this operation to fail.</summary>
    ERROR_GRAPHICS_OPM_INTERNAL_ERROR = 0xC026250B,
    /// <summary>The function failed because the caller passed in an invalid OPM user mode handle.</summary>
    ERROR_GRAPHICS_OPM_INVALID_HANDLE = 0xC026250C,
    /// <summary>A certificate could not be returned because the certificate buffer passed to the function was too small.</summary>
    ERROR_GRAPHICS_PVP_INVALID_CERTIFICATE_LENGTH = 0xC026250E,
    /// <summary>A video output could not be created because the frame buffer is in spanning mode.</summary>
    ERROR_GRAPHICS_OPM_SPANNING_MODE_ENABLED = 0xC026250F,
    /// <summary>A video output could not be created because the frame buffer is in theater mode.</summary>
    ERROR_GRAPHICS_OPM_THEATER_MODE_ENABLED = 0xC0262510,
    /// <summary>The function failed because the display adapter&#39;s Hardware Functionality Scan failed to validate the graphics hardware.</summary>
    ERROR_GRAPHICS_PVP_HFS_FAILED = 0xC0262511,
    /// <summary>The HDCP System Renewability Message passed to this function did not comply with section 5 of the HDCP 1.1 specification.</summary>
    ERROR_GRAPHICS_OPM_INVALID_SRM = 0xC0262512,
    /// <summary>The video output cannot enable the High-bandwidth Digital Content Protection (HDCP) System because it does not support HDCP.</summary>
    ERROR_GRAPHICS_OPM_OUTPUT_DOES_NOT_SUPPORT_HDCP = 0xC0262513,
    /// <summary>The video output cannot enable Analogue Copy Protection (ACP) because it does not support ACP.</summary>
    ERROR_GRAPHICS_OPM_OUTPUT_DOES_NOT_SUPPORT_ACP = 0xC0262514,
    /// <summary>The video output cannot enable the Content Generation Management System Analogue (CGMS-A) protection technology because it does not support CGMS-A.</summary>
    ERROR_GRAPHICS_OPM_OUTPUT_DOES_NOT_SUPPORT_CGMSA = 0xC0262515,
    /// <summary>The IOPMVideoOutput::GetInformation method cannot return the version of the SRM being used because the application never successfully passed an SRM to the video output.</summary>
    ERROR_GRAPHICS_OPM_HDCP_SRM_NEVER_SET = 0xC0262516,
    /// <summary>The IOPMVideoOutput::Configure method cannot enable the specified output protection technology because the output&#39;s screen resolution is too high.</summary>
    ERROR_GRAPHICS_OPM_RESOLUTION_TOO_HIGH = 0xC0262517,
    /// <summary>The IOPMVideoOutput::Configure method cannot enable HDCP because the display adapter&#39;s HDCP hardware is already being used by other physical outputs.</summary>
    ERROR_GRAPHICS_OPM_ALL_HDCP_HARDWARE_ALREADY_IN_USE = 0xC0262518,
    /// <summary>The operating system asynchronously destroyed this OPM video output because the operating system&#39;s state changed. This error typically occurs because the monitor PDO associated with this video output was removed, the monitor PDO associated with this video output was stopped, the video output&#39;s session became a non-console session or the video output&#39;s desktop became an inactive desktop.</summary>
    ERROR_GRAPHICS_OPM_VIDEO_OUTPUT_NO_LONGER_EXISTS = 0xC026251A,
    /// <summary>The method failed because the session is changing its type. No IOPMVideoOutput methods can be called when a session is changing its type. There are currently three types of sessions: console, disconnected and remote.</summary>
    ERROR_GRAPHICS_OPM_SESSION_TYPE_CHANGE_IN_PROGRESS = 0xC026251B,
    /// <summary>Either the IOPMVideoOutput::COPPCompatibleGetInformation, IOPMVideoOutput::GetInformation, or IOPMVideoOutput::Configure method failed. This error is returned when the caller tries to use a COPP specific command while the video output has OPM semantics only.</summary>
    ERROR_GRAPHICS_OPM_VIDEO_OUTPUT_DOES_NOT_HAVE_COPP_SEMANTICS = 0xC026251C,
    /// <summary>The IOPMVideoOutput::GetInformation and IOPMVideoOutput::COPPCompatibleGetInformation methods return this error if the passed in sequence number is not the expected sequence number or the passed in OMAC value is invalid.</summary>
    ERROR_GRAPHICS_OPM_INVALID_INFORMATION_REQUEST = 0xC026251D,
    /// <summary>The method failed because an unexpected error occurred inside of a display driver.</summary>
    ERROR_GRAPHICS_OPM_DRIVER_INTERNAL_ERROR = 0xC026251E,
    /// <summary>Either the IOPMVideoOutput::COPPCompatibleGetInformation, IOPMVideoOutput::GetInformation, or IOPMVideoOutput::Configure method failed. This error is returned when the caller tries to use an OPM specific command while the video output has COPP semantics only.</summary>
    ERROR_GRAPHICS_OPM_VIDEO_OUTPUT_DOES_NOT_HAVE_OPM_SEMANTICS = 0xC026251F,
    /// <summary>The IOPMVideoOutput::COPPCompatibleGetInformation or IOPMVideoOutput::Configure method failed because the display driver does not support the OPM_GET_ACP_AND_CGMSA_SIGNALING and OPM_SET_ACP_AND_CGMSA_SIGNALING GUIDs.</summary>
    ERROR_GRAPHICS_OPM_SIGNALING_NOT_SUPPORTED = 0xC0262520,
    /// <summary>The IOPMVideoOutput::Configure function returns this error code if the passed in sequence number is not the expected sequence number or the passed in OMAC value is invalid.</summary>
    ERROR_GRAPHICS_OPM_INVALID_CONFIGURATION_REQUEST = 0xC0262521,
    /// <summary>The monitor connected to the specified video output does not have an I2C bus.</summary>
    ERROR_GRAPHICS_I2C_NOT_SUPPORTED = 0xC0262580,
    /// <summary>No device on the I2C bus has the specified address.</summary>
    ERROR_GRAPHICS_I2C_DEVICE_DOES_NOT_EXIST = 0xC0262581,
    /// <summary>An error occurred while transmitting data to the device on the I2C bus.</summary>
    ERROR_GRAPHICS_I2C_ERROR_TRANSMITTING_DATA = 0xC0262582,
    /// <summary>An error occurred while receiving data from the device on the I2C bus.</summary>
    ERROR_GRAPHICS_I2C_ERROR_RECEIVING_DATA = 0xC0262583,
    /// <summary>The monitor does not support the specified VCP code.</summary>
    ERROR_GRAPHICS_DDCCI_VCP_NOT_SUPPORTED = 0xC0262584,
    /// <summary>The data received from the monitor is invalid.</summary>
    ERROR_GRAPHICS_DDCCI_INVALID_DATA = 0xC0262585,
    /// <summary>The function failed because a monitor returned an invalid Timing Status byte when the operating system used the DDC/CI Get Timing Report &amp; Timing Message command to get a timing report from a monitor.</summary>
    ERROR_GRAPHICS_DDCCI_MONITOR_RETURNED_INVALID_TIMING_STATUS_BYTE = 0xC0262586,
    /// <summary>The monitor returned a DDC/CI capabilities string which did not comply with the ACCESS.bus 3.0, DDC/CI 1.1, or MCCS 2 Revision 1 specification.</summary>
    ERROR_GRAPHICS_MCA_INVALID_CAPABILITIES_STRING = 0xC0262587,
    /// <summary>An internal Monitor Configuration API error occured.</summary>
    ERROR_GRAPHICS_MCA_INTERNAL_ERROR = 0xC0262588,
    /// <summary>An operation failed because a DDC/CI message had an invalid value in its command field.</summary>
    ERROR_GRAPHICS_DDCCI_INVALID_MESSAGE_COMMAND = 0xC0262589,
    /// <summary>An error occurred because the field length of a DDC/CI message contained an invalid value.</summary>
    ERROR_GRAPHICS_DDCCI_INVALID_MESSAGE_LENGTH = 0xC026258A,
    /// <summary>An error occurred because the checksum field in a DDC/CI message did not match the message&#39;s computed checksum value. This error implies that the data was corrupted while it was being transmitted from a monitor to a computer.</summary>
    ERROR_GRAPHICS_DDCCI_INVALID_MESSAGE_CHECKSUM = 0xC026258B,
    /// <summary>This function failed because an invalid monitor handle was passed to it.</summary>
    ERROR_GRAPHICS_INVALID_PHYSICAL_MONITOR_HANDLE = 0xC026258C,
    /// <summary>The operating system asynchronously destroyed the monitor which corresponds to this handle because the operating system&#39;s state changed. This error typically occurs because the monitor PDO associated with this handle was removed, the monitor PDO associated with this handle was stopped, or a display mode change occurred. A display mode change occurs when windows sends a WM_DISPLAYCHANGE windows message to applications.</summary>
    ERROR_GRAPHICS_MONITOR_NO_LONGER_EXISTS = 0xC026258D,
    /// <summary>A continuous VCP code&#39;s current value is greater than its maximum value. This error code indicates that a monitor returned an invalid value.</summary>
    ERROR_GRAPHICS_DDCCI_CURRENT_CURRENT_VALUE_GREATER_THAN_MAXIMUM_VALUE = 0xC02625D8,
    /// <summary>The monitor&#39;s VCP Version (0xDF) VCP code returned an invalid version value.</summary>
    ERROR_GRAPHICS_MCA_INVALID_VCP_VERSION = 0xC02625D9,
    /// <summary>The monitor does not comply with the MCCS specification it claims to support.</summary>
    ERROR_GRAPHICS_MCA_MONITOR_VIOLATES_MCCS_SPECIFICATION = 0xC02625DA,
    /// <summary>The MCCS version in a monitor&#39;s mccs_ver capability does not match the MCCS version the monitor reports when the VCP Version (0xDF) VCP code is used.</summary>
    ERROR_GRAPHICS_MCA_MCCS_VERSION_MISMATCH = 0xC02625DB,
    /// <summary>The Monitor Configuration API only works with monitors which support the MCCS 1.0 specification, MCCS 2.0 specification or the MCCS 2.0 Revision 1 specification.</summary>
    ERROR_GRAPHICS_MCA_UNSUPPORTED_MCCS_VERSION = 0xC02625DC,
    /// <summary>The monitor returned an invalid monitor technology type. CRT, Plasma and LCD (TFT) are examples of monitor technology types. This error implies that the monitor violated the MCCS 2.0 or MCCS 2.0 Revision 1 specification.</summary>
    ERROR_GRAPHICS_MCA_INVALID_TECHNOLOGY_TYPE_RETURNED = 0xC02625DE,
    /// <summary>SetMonitorColorTemperature()&#39;s caller passed a color temperature to it which the current monitor did not support. This error implies that the monitor violated the MCCS 2.0 or MCCS 2.0 Revision 1 specification.</summary>
    ERROR_GRAPHICS_MCA_UNSUPPORTED_COLOR_TEMPERATURE = 0xC02625DF,
    /// <summary>This function can only be used if a program is running in the local console session. It cannot be used if the program is running on a remote desktop session or on a terminal server session.</summary>
    ERROR_GRAPHICS_ONLY_CONSOLE_SESSION_SUPPORTED = 0xC02625E0,
    /// <summary>This function cannot find an actual GDI display device which corresponds to the specified GDI display device name.</summary>
    ERROR_GRAPHICS_NO_DISPLAY_DEVICE_CORRESPONDS_TO_NAME = 0xC02625E1,
    /// <summary>The function failed because the specified GDI display device was not attached to the Windows desktop.</summary>
    ERROR_GRAPHICS_DISPLAY_DEVICE_NOT_ATTACHED_TO_DESKTOP = 0xC02625E2,
    /// <summary>This function does not support GDI mirroring display devices because GDI mirroring display devices do not have any physical monitors associated with them.</summary>
    ERROR_GRAPHICS_MIRRORING_DEVICES_NOT_SUPPORTED = 0xC02625E3,
    /// <summary>The function failed because an invalid pointer parameter was passed to it. A pointer parameter is invalid if it is NULL, points to an invalid address, points to a kernel mode address, or is not correctly aligned.</summary>
    ERROR_GRAPHICS_INVALID_POINTER = 0xC02625E4,
    /// <summary>The function failed because the specified GDI device did not have any monitors associated with it.</summary>
    ERROR_GRAPHICS_NO_MONITORS_CORRESPOND_TO_DISPLAY_DEVICE = 0xC02625E5,
    /// <summary>An array passed to the function cannot hold all of the data that the function must copy into the array.</summary>
    ERROR_GRAPHICS_PARAMETER_ARRAY_TOO_SMALL = 0xC02625E6,
    /// <summary>An internal error caused an operation to fail.</summary>
    ERROR_GRAPHICS_INTERNAL_ERROR = 0xC02625E7,
    /// <summary>The function failed because the current session is changing its type. This function cannot be called when the current session is changing its type. There are currently three types of sessions: console, disconnected and remote.</summary>
    ERROR_GRAPHICS_SESSION_TYPE_CHANGE_IN_PROGRESS = 0xC02605E8,
    /// <summary>This is an error mask to convert TPM hardware errors to win errors.</summary>
    TPM_E_ERROR_MASK = 0x80280000,
    /// <summary>Authentication failed.</summary>
    TPM_E_AUTHFAIL = 0x80280001,
    /// <summary>The index to a PCR, DIR or other register is incorrect.</summary>
    TPM_E_BADINDEX = 0x80280002,
    /// <summary>One or more parameter is bad.</summary>
    TPM_E_BAD_PARAMETER = 0x80280003,
    /// <summary>An operation completed successfully but the auditing of that operation failed.</summary>
    TPM_E_AUDITFAILURE = 0x80280004,
    /// <summary>The clear disable flag is set and all clear operations now require physical access.</summary>
    TPM_E_CLEAR_DISABLED = 0x80280005,
    /// <summary>Activate the Trusted Platform Module (TPM).</summary>
    TPM_E_DEACTIVATED = 0x80280006,
    /// <summary>Enable the Trusted Platform Module (TPM).</summary>
    TPM_E_DISABLED = 0x80280007,
    /// <summary>The target command has been disabled.</summary>
    TPM_E_DISABLED_CMD = 0x80280008,
    /// <summary>The operation failed.</summary>
    TPM_E_FAIL = 0x80280009,
    /// <summary>The ordinal was unknown or inconsistent.</summary>
    TPM_E_BAD_ORDINAL = 0x8028000A,
    /// <summary>The ability to install an owner is disabled.</summary>
    TPM_E_INSTALL_DISABLED = 0x8028000B,
    /// <summary>The key handle cannot be interpreted.</summary>
    TPM_E_INVALID_KEYHANDLE = 0x8028000C,
    /// <summary>The key handle points to an invalid key.</summary>
    TPM_E_KEYNOTFOUND = 0x8028000D,
    /// <summary>Unacceptable encryption scheme.</summary>
    TPM_E_INAPPROPRIATE_ENC = 0x8028000E,
    /// <summary>Migration authorization failed.</summary>
    TPM_E_MIGRATEFAIL = 0x8028000F,
    /// <summary>PCR information could not be interpreted.</summary>
    TPM_E_INVALID_PCR_INFO = 0x80280010,
    /// <summary>No room to load key.</summary>
    TPM_E_NOSPACE = 0x80280011,
    /// <summary>There is no Storage Root Key (SRK) set.</summary>
    TPM_E_NOSRK = 0x80280012,
    /// <summary>An encrypted blob is invalid or was not created by this TPM.</summary>
    TPM_E_NOTSEALED_BLOB = 0x80280013,
    /// <summary>The Trusted Platform Module (TPM) already has an owner.</summary>
    TPM_E_OWNER_SET = 0x80280014,
    /// <summary>The TPM has insufficient internal resources to perform the requested action.</summary>
    TPM_E_RESOURCES = 0x80280015,
    /// <summary>A random string was too short.</summary>
    TPM_E_SHORTRANDOM = 0x80280016,
    /// <summary>The TPM does not have the space to perform the operation.</summary>
    TPM_E_SIZE = 0x80280017,
    /// <summary>The named PCR value does not match the current PCR value.</summary>
    TPM_E_WRONGPCRVAL = 0x80280018,
    /// <summary>The paramSize argument to the command has the incorrect value .</summary>
    TPM_E_BAD_PARAM_SIZE = 0x80280019,
    /// <summary>There is no existing SHA-1 thread.</summary>
    TPM_E_SHA_THREAD = 0x8028001A,
    /// <summary>The calculation is unable to proceed because the existing SHA-1 thread has already encountered an error.</summary>
    TPM_E_SHA_ERROR = 0x8028001B,
    /// <summary>The TPM hardware device reported a failure during its internal self test. Try restarting the computer to resolve the problem. If the problem continues, you might need to replace your TPM hardware or motherboard.</summary>
    TPM_E_FAILEDSELFTEST = 0x8028001C,
    /// <summary>The authorization for the second key in a 2 key function failed authorization.</summary>
    TPM_E_AUTH2FAIL = 0x8028001D,
    /// <summary>The tag value sent to for a command is invalid.</summary>
    TPM_E_BADTAG = 0x8028001E,
    /// <summary>An IO error occurred transmitting information to the TPM.</summary>
    TPM_E_IOERROR = 0x8028001F,
    /// <summary>The encryption process had a problem.</summary>
    TPM_E_ENCRYPT_ERROR = 0x80280020,
    /// <summary>The decryption process did not complete.</summary>
    TPM_E_DECRYPT_ERROR = 0x80280021,
    /// <summary>An invalid handle was used.</summary>
    TPM_E_INVALID_AUTHHANDLE = 0x80280022,
    /// <summary>The TPM does not have an Endorsement Key (EK) installed.</summary>
    TPM_E_NO_ENDORSEMENT = 0x80280023,
    /// <summary>The usage of a key is not allowed.</summary>
    TPM_E_INVALID_KEYUSAGE = 0x80280024,
    /// <summary>The submitted entity type is not allowed.</summary>
    TPM_E_WRONG_ENTITYTYPE = 0x80280025,
    /// <summary>The command was received in the wrong sequence relative to TPM_Init and a subsequent TPM_Startup.</summary>
    TPM_E_INVALID_POSTINIT = 0x80280026,
    /// <summary>Signed data cannot include additional DER information.</summary>
    TPM_E_INAPPROPRIATE_SIG = 0x80280027,
    /// <summary>The key properties in TPM_KEY_PARMs are not supported by this TPM.</summary>
    TPM_E_BAD_KEY_PROPERTY = 0x80280028,
    /// <summary>The migration properties of this key are incorrect.</summary>
    TPM_E_BAD_MIGRATION = 0x80280029,
    /// <summary>The signature or encryption scheme for this key is incorrect or not permitted in this situation.</summary>
    TPM_E_BAD_SCHEME = 0x8028002A,
    /// <summary>The size of the data (or blob) parameter is bad or inconsistent with the referenced key.</summary>
    TPM_E_BAD_DATASIZE = 0x8028002B,
    /// <summary>A mode parameter is bad, such as capArea or subCapArea for TPM_GetCapability, phsicalPresence parameter for TPM_PhysicalPresence, or migrationType for TPM_CreateMigrationBlob.</summary>
    TPM_E_BAD_MODE = 0x8028002C,
    /// <summary>Either the physicalPresence or physicalPresenceLock bits have the wrong value.</summary>
    TPM_E_BAD_PRESENCE = 0x8028002D,
    /// <summary>The TPM cannot perform this version of the capability.</summary>
    TPM_E_BAD_VERSION = 0x8028002E,
    /// <summary>The TPM does not allow for wrapped transport sessions.</summary>
    TPM_E_NO_WRAP_TRANSPORT = 0x8028002F,
    /// <summary>TPM audit construction failed and the underlying command was returning a failure code also.</summary>
    TPM_E_AUDITFAIL_UNSUCCESSFUL = 0x80280030,
    /// <summary>TPM audit construction failed and the underlying command was returning success.</summary>
    TPM_E_AUDITFAIL_SUCCESSFUL = 0x80280031,
    /// <summary>Attempt to reset a PCR register that does not have the resettable attribute.</summary>
    TPM_E_NOTRESETABLE = 0x80280032,
    /// <summary>Attempt to reset a PCR register that requires locality and locality modifier not part of command transport.</summary>
    TPM_E_NOTLOCAL = 0x80280033,
    /// <summary>Make identity blob not properly typed.</summary>
    TPM_E_BAD_TYPE = 0x80280034,
    /// <summary>When saving context identified resource type does not match actual resource.</summary>
    TPM_E_INVALID_RESOURCE = 0x80280035,
    /// <summary>The TPM is attempting to execute a command only available when in FIPS mode.</summary>
    TPM_E_NOTFIPS = 0x80280036,
    /// <summary>The command is attempting to use an invalid family ID.</summary>
    TPM_E_INVALID_FAMILY = 0x80280037,
    /// <summary>The permission to manipulate the NV storage is not available.</summary>
    TPM_E_NO_NV_PERMISSION = 0x80280038,
    /// <summary>The operation requires a signed command.</summary>
    TPM_E_REQUIRES_SIGN = 0x80280039,
    /// <summary>Wrong operation to load an NV key.</summary>
    TPM_E_KEY_NOTSUPPORTED = 0x8028003A,
    /// <summary>NV_LoadKey blob requires both owner and blob authorization.</summary>
    TPM_E_AUTH_CONFLICT = 0x8028003B,
    /// <summary>The NV area is locked and not writtable.</summary>
    TPM_E_AREA_LOCKED = 0x8028003C,
    /// <summary>The locality is incorrect for the attempted operation.</summary>
    TPM_E_BAD_LOCALITY = 0x8028003D,
    /// <summary>The NV area is read only and can&#39;t be written to.</summary>
    TPM_E_READ_ONLY = 0x8028003E,
    /// <summary>There is no protection on the write to the NV area.</summary>
    TPM_E_PER_NOWRITE = 0x8028003F,
    /// <summary>The family count value does not match.</summary>
    TPM_E_FAMILYCOUNT = 0x80280040,
    /// <summary>The NV area has already been written to.</summary>
    TPM_E_WRITE_LOCKED = 0x80280041,
    /// <summary>The NV area attributes conflict.</summary>
    TPM_E_BAD_ATTRIBUTES = 0x80280042,
    /// <summary>The structure tag and version are invalid or inconsistent.</summary>
    TPM_E_INVALID_STRUCTURE = 0x80280043,
    /// <summary>The key is under control of the TPM Owner and can only be evicted by the TPM Owner.</summary>
    TPM_E_KEY_OWNER_CONTROL = 0x80280044,
    /// <summary>The counter handle is incorrect.</summary>
    TPM_E_BAD_COUNTER = 0x80280045,
    /// <summary>The write is not a complete write of the area.</summary>
    TPM_E_NOT_FULLWRITE = 0x80280046,
    /// <summary>The gap between saved context counts is too large.</summary>
    TPM_E_CONTEXT_GAP = 0x80280047,
    /// <summary>The maximum number of NV writes without an owner has been exceeded.</summary>
    TPM_E_MAXNVWRITES = 0x80280048,
    /// <summary>No operator AuthData value is set.</summary>
    TPM_E_NOOPERATOR = 0x80280049,
    /// <summary>The resource pointed to by context is not loaded.</summary>
    TPM_E_RESOURCEMISSING = 0x8028004A,
    /// <summary>The delegate administration is locked.</summary>
    TPM_E_DELEGATE_LOCK = 0x8028004B,
    /// <summary>Attempt to manage a family other then the delegated family.</summary>
    TPM_E_DELEGATE_FAMILY = 0x8028004C,
    /// <summary>Delegation table management not enabled.</summary>
    TPM_E_DELEGATE_ADMIN = 0x8028004D,
    /// <summary>There was a command executed outside of an exclusive transport session.</summary>
    TPM_E_TRANSPORT_NOTEXCLUSIVE = 0x8028004E,
    /// <summary>Attempt to context save a owner evict controlled key.</summary>
    TPM_E_OWNER_CONTROL = 0x8028004F,
    /// <summary>The DAA command has no resources availble to execute the command.</summary>
    TPM_E_DAA_RESOURCES = 0x80280050,
    /// <summary>The consistency check on DAA parameter inputData0 has failed.</summary>
    TPM_E_DAA_INPUT_DATA0 = 0x80280051,
    /// <summary>The consistency check on DAA parameter inputData1 has failed.</summary>
    TPM_E_DAA_INPUT_DATA1 = 0x80280052,
    /// <summary>The consistency check on DAA_issuerSettings has failed.</summary>
    TPM_E_DAA_ISSUER_SETTINGS = 0x80280053,
    /// <summary>The consistency check on DAA_tpmSpecific has failed.</summary>
    TPM_E_DAA_TPM_SETTINGS = 0x80280054,
    /// <summary>The atomic process indicated by the submitted DAA command is not the expected process.</summary>
    TPM_E_DAA_STAGE = 0x80280055,
    /// <summary>The issuer&#39;s validity check has detected an inconsistency.</summary>
    TPM_E_DAA_ISSUER_VALIDITY = 0x80280056,
    /// <summary>The consistency check on w has failed.</summary>
    TPM_E_DAA_WRONG_W = 0x80280057,
    /// <summary>The handle is incorrect.</summary>
    TPM_E_BAD_HANDLE = 0x80280058,
    /// <summary>Delegation is not correct.</summary>
    TPM_E_BAD_DELEGATE = 0x80280059,
    /// <summary>The context blob is invalid.</summary>
    TPM_E_BADCONTEXT = 0x8028005A,
    /// <summary>Too many contexts held by the TPM.</summary>
    TPM_E_TOOMANYCONTEXTS = 0x8028005B,
    /// <summary>Migration authority signature validation failure.</summary>
    TPM_E_MA_TICKET_SIGNATURE = 0x8028005C,
    /// <summary>Migration destination not authenticated.</summary>
    TPM_E_MA_DESTINATION = 0x8028005D,
    /// <summary>Migration source incorrect.</summary>
    TPM_E_MA_SOURCE = 0x8028005E,
    /// <summary>Incorrect migration authority.</summary>
    TPM_E_MA_AUTHORITY = 0x8028005F,
    /// <summary>Attempt to revoke the EK and the EK is not revocable.</summary>
    TPM_E_PERMANENTEK = 0x80280061,
    /// <summary>Bad signature of CMK ticket.</summary>
    TPM_E_BAD_SIGNATURE = 0x80280062,
    /// <summary>There is no room in the context list for additional contexts.</summary>
    TPM_E_NOCONTEXTSPACE = 0x80280063,
    /// <summary>The command was blocked.</summary>
    TPM_E_COMMAND_BLOCKED = 0x80280400,
    /// <summary>The specified handle was not found.</summary>
    TPM_E_INVALID_HANDLE = 0x80280401,
    /// <summary>The TPM returned a duplicate handle and the command needs to be resubmitted.</summary>
    TPM_E_DUPLICATE_VHANDLE = 0x80280402,
    /// <summary>The command within the transport was blocked.</summary>
    TPM_E_EMBEDDED_COMMAND_BLOCKED = 0x80280403,
    /// <summary>The command within the transport is not supported.</summary>
    TPM_E_EMBEDDED_COMMAND_UNSUPPORTED = 0x80280404,
    /// <summary>The TPM is too busy to respond to the command immediately, but the command could be resubmitted at a later time.</summary>
    TPM_E_RETRY = 0x80280800,
    /// <summary>SelfTestFull has not been run.</summary>
    TPM_E_NEEDS_SELFTEST = 0x80280801,
    /// <summary>The TPM is currently executing a full selftest.</summary>
    TPM_E_DOING_SELFTEST = 0x80280802,
    /// <summary>The TPM is defending against dictionary attacks and is in a time-out period.</summary>
    TPM_E_DEFEND_LOCK_RUNNING = 0x80280803,
    /// <summary>An internal software error has been detected.</summary>
    TBS_E_INTERNAL_ERROR = 0x80284001,
    /// <summary>One or more input parameters is bad.</summary>
    TBS_E_BAD_PARAMETER = 0x80284002,
    /// <summary>A specified output pointer is bad.</summary>
    TBS_E_INVALID_OUTPUT_POINTER = 0x80284003,
    /// <summary>The specified context handle does not refer to a valid context.</summary>
    TBS_E_INVALID_CONTEXT = 0x80284004,
    /// <summary>A specified output buffer is too small.</summary>
    TBS_E_INSUFFICIENT_BUFFER = 0x80284005,
    /// <summary>An error occurred while communicating with the TPM.</summary>
    TBS_E_IOERROR = 0x80284006,
    /// <summary>One or more context parameters is invalid.</summary>
    TBS_E_INVALID_CONTEXT_PARAM = 0x80284007,
    /// <summary>The TBS service is not running and could not be started.</summary>
    TBS_E_SERVICE_NOT_RUNNING = 0x80284008,
    /// <summary>A new context could not be created because there are too many open contexts.</summary>
    TBS_E_TOO_MANY_TBS_CONTEXTS = 0x80284009,
    /// <summary>A new virtual resource could not be created because there are too many open virtual resources.</summary>
    TBS_E_TOO_MANY_RESOURCES = 0x8028400A,
    /// <summary>The TBS service has been started but is not yet running.</summary>
    TBS_E_SERVICE_START_PENDING = 0x8028400B,
    /// <summary>The physical presence interface is not supported.</summary>
    TBS_E_PPI_NOT_SUPPORTED = 0x8028400C,
    /// <summary>The command was canceled.</summary>
    TBS_E_COMMAND_CANCELED = 0x8028400D,
    /// <summary>The input or output buffer is too large.</summary>
    TBS_E_BUFFER_TOO_LARGE = 0x8028400E,
    /// <summary>A compatible Trusted Platform Module (TPM) Security Device cannot be found on this computer.</summary>
    TBS_E_TPM_NOT_FOUND = 0x8028400F,
    /// <summary>The TBS service has been disabled.</summary>
    TBS_E_SERVICE_DISABLED = 0x80284010,
    /// <summary>No TCG event log is available.</summary>
    TBS_E_NO_EVENT_LOG = 0x80284011,
    /// <summary>The command buffer is not in the correct state.</summary>
    TPMAPI_E_INVALID_STATE = 0x80290100,
    /// <summary>The command buffer does not contain enough data to satisfy the request.</summary>
    TPMAPI_E_NOT_ENOUGH_DATA = 0x80290101,
    /// <summary>The command buffer cannot contain any more data.</summary>
    TPMAPI_E_TOO_MUCH_DATA = 0x80290102,
    /// <summary>One or more output parameters was NULL or invalid.</summary>
    TPMAPI_E_INVALID_OUTPUT_POINTER = 0x80290103,
    /// <summary>One or more input parameters is invalid.</summary>
    TPMAPI_E_INVALID_PARAMETER = 0x80290104,
    /// <summary>Not enough memory was available to satisfy the request.</summary>
    TPMAPI_E_OUT_OF_MEMORY = 0x80290105,
    /// <summary>The specified buffer was too small.</summary>
    TPMAPI_E_BUFFER_TOO_SMALL = 0x80290106,
    /// <summary>An internal error was detected.</summary>
    TPMAPI_E_INTERNAL_ERROR = 0x80290107,
    /// <summary>The caller does not have the appropriate rights to perform the requested operation.</summary>
    TPMAPI_E_ACCESS_DENIED = 0x80290108,
    /// <summary>The specified authorization information was invalid.</summary>
    TPMAPI_E_AUTHORIZATION_FAILED = 0x80290109,
    /// <summary>The specified context handle was not valid.</summary>
    TPMAPI_E_INVALID_CONTEXT_HANDLE = 0x8029010A,
    /// <summary>An error occurred while communicating with the TBS.</summary>
    TPMAPI_E_TBS_COMMUNICATION_ERROR = 0x8029010B,
    /// <summary>The TPM returned an unexpected result.</summary>
    TPMAPI_E_TPM_COMMAND_ERROR = 0x8029010C,
    /// <summary>The message was too large for the encoding scheme.</summary>
    TPMAPI_E_MESSAGE_TOO_LARGE = 0x8029010D,
    /// <summary>The encoding in the blob was not recognized.</summary>
    TPMAPI_E_INVALID_ENCODING = 0x8029010E,
    /// <summary>The key size is not valid.</summary>
    TPMAPI_E_INVALID_KEY_SIZE = 0x8029010F,
    /// <summary>The encryption operation failed.</summary>
    TPMAPI_E_ENCRYPTION_FAILED = 0x80290110,
    /// <summary>The key parameters structure was not valid</summary>
    TPMAPI_E_INVALID_KEY_PARAMS = 0x80290111,
    /// <summary>The requested supplied data does not appear to be a valid migration authorization blob.</summary>
    TPMAPI_E_INVALID_MIGRATION_AUTHORIZATION_BLOB = 0x80290112,
    /// <summary>The specified PCR index was invalid</summary>
    TPMAPI_E_INVALID_PCR_INDEX = 0x80290113,
    /// <summary>The data given does not appear to be a valid delegate blob.</summary>
    TPMAPI_E_INVALID_DELEGATE_BLOB = 0x80290114,
    /// <summary>One or more of the specified context parameters was not valid.</summary>
    TPMAPI_E_INVALID_CONTEXT_PARAMS = 0x80290115,
    /// <summary>The data given does not appear to be a valid key blob</summary>
    TPMAPI_E_INVALID_KEY_BLOB = 0x80290116,
    /// <summary>The specified PCR data was invalid.</summary>
    TPMAPI_E_INVALID_PCR_DATA = 0x80290117,
    /// <summary>The format of the owner auth data was invalid.</summary>
    TPMAPI_E_INVALID_OWNER_AUTH = 0x80290118,
    /// <summary>The random number generated did not pass FIPS RNG check.</summary>
    TPMAPI_E_FIPS_RNG_CHECK_FAILED = 0x80290119,
    /// <summary>The TCG Event Log does not contain any data.</summary>
    TPMAPI_E_EMPTY_TCG_LOG = 0x8029011A,
    /// <summary>An entry in the TCG Event Log was invalid.</summary>
    TPMAPI_E_INVALID_TCG_LOG_ENTRY = 0x8029011B,
    /// <summary>A TCG Separator was not found.</summary>
    TPMAPI_E_TCG_SEPARATOR_ABSENT = 0x8029011C,
    /// <summary>A digest value in a TCG Log entry did not match hashed data.</summary>
    TPMAPI_E_TCG_INVALID_DIGEST_ENTRY = 0x8029011D,
    /// <summary>The specified buffer was too small.</summary>
    TBSIMP_E_BUFFER_TOO_SMALL = 0x80290200,
    /// <summary>The context could not be cleaned up.</summary>
    TBSIMP_E_CLEANUP_FAILED = 0x80290201,
    /// <summary>The specified context handle is invalid.</summary>
    TBSIMP_E_INVALID_CONTEXT_HANDLE = 0x80290202,
    /// <summary>An invalid context parameter was specified.</summary>
    TBSIMP_E_INVALID_CONTEXT_PARAM = 0x80290203,
    /// <summary>An error occurred while communicating with the TPM</summary>
    TBSIMP_E_TPM_ERROR = 0x80290204,
    /// <summary>No entry with the specified key was found.</summary>
    TBSIMP_E_HASH_BAD_KEY = 0x80290205,
    /// <summary>The specified virtual handle matches a virtual handle already in use.</summary>
    TBSIMP_E_DUPLICATE_VHANDLE = 0x80290206,
    /// <summary>The pointer to the returned handle location was NULL or invalid</summary>
    TBSIMP_E_INVALID_OUTPUT_POINTER = 0x80290207,
    /// <summary>One or more parameters is invalid</summary>
    TBSIMP_E_INVALID_PARAMETER = 0x80290208,
    /// <summary>The RPC subsystem could not be initialized.</summary>
    TBSIMP_E_RPC_INIT_FAILED = 0x80290209,
    /// <summary>The TBS scheduler is not running.</summary>
    TBSIMP_E_SCHEDULER_NOT_RUNNING = 0x8029020A,
    /// <summary>The command was canceled.</summary>
    TBSIMP_E_COMMAND_CANCELED = 0x8029020B,
    /// <summary>There was not enough memory to fulfill the request</summary>
    TBSIMP_E_OUT_OF_MEMORY = 0x8029020C,
    /// <summary>The specified list is empty, or the iteration has reached the end of the list.</summary>
    TBSIMP_E_LIST_NO_MORE_ITEMS = 0x8029020D,
    /// <summary>The specified item was not found in the list.</summary>
    TBSIMP_E_LIST_NOT_FOUND = 0x8029020E,
    /// <summary>The TPM does not have enough space to load the requested resource.</summary>
    TBSIMP_E_NOT_ENOUGH_SPACE = 0x8029020F,
    /// <summary>There are too many TPM contexts in use.</summary>
    TBSIMP_E_NOT_ENOUGH_TPM_CONTEXTS = 0x80290210,
    /// <summary>The TPM command failed.</summary>
    TBSIMP_E_COMMAND_FAILED = 0x80290211,
    /// <summary>The TBS does not recognize the specified ordinal.</summary>
    TBSIMP_E_UNKNOWN_ORDINAL = 0x80290212,
    /// <summary>The requested resource is no longer available.</summary>
    TBSIMP_E_RESOURCE_EXPIRED = 0x80290213,
    /// <summary>The resource type did not match.</summary>
    TBSIMP_E_INVALID_RESOURCE = 0x80290214,
    /// <summary>No resources can be unloaded.</summary>
    TBSIMP_E_NOTHING_TO_UNLOAD = 0x80290215,
    /// <summary>No new entries can be added to the hash table.</summary>
    TBSIMP_E_HASH_TABLE_FULL = 0x80290216,
    /// <summary>A new TBS context could not be created because there are too many open contexts.</summary>
    TBSIMP_E_TOO_MANY_TBS_CONTEXTS = 0x80290217,
    /// <summary>A new virtual resource could not be created because there are too many open virtual resources.</summary>
    TBSIMP_E_TOO_MANY_RESOURCES = 0x80290218,
    /// <summary>The physical presence interface is not supported.</summary>
    TBSIMP_E_PPI_NOT_SUPPORTED = 0x80290219,
    /// <summary>TBS is not compatible with the version of TPM found on the system.</summary>
    TBSIMP_E_TPM_INCOMPATIBLE = 0x8029021A,
    /// <summary>No TCG event log is available.</summary>
    TBSIMP_E_NO_EVENT_LOG = 0x8029021B,
    /// <summary>A general error was detected when attempting to acquire the BIOS&#39;s response to a Physical Presence command.</summary>
    TPM_E_PPI_ACPI_FAILURE = 0x80290300,
    /// <summary>The user failed to confirm the TPM operation request.</summary>
    TPM_E_PPI_USER_ABORT = 0x80290301,
    /// <summary>The BIOS failure prevented the successful execution of the requested TPM operation (e.g. invalid TPM operation request, BIOS communication error with the TPM).</summary>
    TPM_E_PPI_BIOS_FAILURE = 0x80290302,
    /// <summary>The BIOS does not support the physical presence interface.</summary>
    TPM_E_PPI_NOT_SUPPORTED = 0x80290303,
    /// <summary>Data Collector Set was not found.</summary>
    PLA_E_DCS_NOT_FOUND = 0x80300002,
    /// <summary>The Data Collector Set or one of its dependencies is already in use.</summary>
    PLA_E_DCS_IN_USE = 0x803000AA,
    /// <summary>Unable to start Data Collector Set because there are too many folders.</summary>
    PLA_E_TOO_MANY_FOLDERS = 0x80300045,
    /// <summary>Not enough free disk space to start Data Collector Set.</summary>
    PLA_E_NO_MIN_DISK = 0x80300070,
    /// <summary>Data Collector Set already exists.</summary>
    PLA_E_DCS_ALREADY_EXISTS = 0x803000B7,
    /// <summary>Property value will be ignored.</summary>
    PLA_S_PROPERTY_IGNORED = 0x00300100,
    /// <summary>Property value conflict.</summary>
    PLA_E_PROPERTY_CONFLICT = 0x80300101,
    /// <summary>The current configuration for this Data Collector Set requires that it contain exactly one Data Collector.</summary>
    PLA_E_DCS_SINGLETON_REQUIRED = 0x80300102,
    /// <summary>A user account is required in order to commit the current Data Collector Set properties.</summary>
    PLA_E_CREDENTIALS_REQUIRED = 0x80300103,
    /// <summary>Data Collector Set is not running.</summary>
    PLA_E_DCS_NOT_RUNNING = 0x80300104,
    /// <summary>A conflict was detected in the list of include/exclude APIs. Do not specify the same API in both the include list and the exclude list.</summary>
    PLA_E_CONFLICT_INCL_EXCL_API = 0x80300105,
    /// <summary>The executable path you have specified refers to a network share or UNC path.</summary>
    PLA_E_NETWORK_EXE_NOT_VALID = 0x80300106,
    /// <summary>The executable path you have specified is already configured for API tracing.</summary>
    PLA_E_EXE_ALREADY_CONFIGURED = 0x80300107,
    /// <summary>The executable path you have specified does not exist. Verify that the specified path is correct.</summary>
    PLA_E_EXE_PATH_NOT_VALID = 0x80300108,
    /// <summary>Data Collector already exists.</summary>
    PLA_E_DC_ALREADY_EXISTS = 0x80300109,
    /// <summary>The wait for the Data Collector Set start notification has timed out.</summary>
    PLA_E_DCS_START_WAIT_TIMEOUT = 0x8030010A,
    /// <summary>The wait for the Data Collector to start has timed out.</summary>
    PLA_E_DC_START_WAIT_TIMEOUT = 0x8030010B,
    /// <summary>The wait for the report generation tool to finish has timed out.</summary>
    PLA_E_REPORT_WAIT_TIMEOUT = 0x8030010C,
    /// <summary>Duplicate items are not allowed.</summary>
    PLA_E_NO_DUPLICATES = 0x8030010D,
    /// <summary>When specifying the executable that you want to trace, you must specify a full path to the executable and not just a filename.</summary>
    PLA_E_EXE_FULL_PATH_REQUIRED = 0x8030010E,
    /// <summary>The session name provided is invalid.</summary>
    PLA_E_INVALID_SESSION_NAME = 0x8030010F,
    /// <summary>The Event Log channel Microsoft-Windows-Diagnosis-PLA/Operational must be enabled to perform this operation.</summary>
    PLA_E_PLA_CHANNEL_NOT_ENABLED = 0x80300110,
    /// <summary>The Event Log channel Microsoft-Windows-TaskScheduler must be enabled to perform this operation.</summary>
    PLA_E_TASKSCHED_CHANNEL_NOT_ENABLED = 0x80300111,
    /// <summary>The execution of the Rules Manager failed.</summary>
    PLA_E_RULES_MANAGER_FAILED = 0x80300112,
    /// <summary>An error occurred while attempting to compress or extract the data.</summary>
    PLA_E_CABAPI_FAILURE = 0x80300113,
    /// <summary>This drive is locked by BitLocker Drive Encryption. You must unlock this drive from Control Panel.</summary>
    FVE_E_LOCKED_VOLUME = 0x80310000,
    /// <summary>This drive is not encrypted.</summary>
    FVE_E_NOT_ENCRYPTED = 0x80310001,
    /// <summary>The BIOS did not correctly communicate with the Trusted Platform Module (TPM). Contact the computer manufacturer for BIOS upgrade instructions.</summary>
    FVE_E_NO_TPM_BIOS = 0x80310002,
    /// <summary>The BIOS did not correctly communicate with the master boot record (MBR). Contact the computer manufacturer for BIOS upgrade instructions.</summary>
    FVE_E_NO_MBR_METRIC = 0x80310003,
    /// <summary>A required TPM measurement is missing. If there is a bootable CD or DVD in your computer, remove it, restart the computer, and turn on BitLocker again. If the problem persists, ensure the master boot record is up to date.</summary>
    FVE_E_NO_BOOTSECTOR_METRIC = 0x80310004,
    /// <summary>The boot sector of this drive is not compatible with BitLocker Drive Encryption. Use the Bootrec.exe tool in the Windows Recovery Environment to update or repair the boot manager (BOOTMGR).</summary>
    FVE_E_NO_BOOTMGR_METRIC = 0x80310005,
    /// <summary>The boot manager of this operating system is not compatible with BitLocker Drive Encryption. Use the Bootrec.exe tool in the Windows Recovery Environment to update or repair the boot manager (BOOTMGR).</summary>
    FVE_E_WRONG_BOOTMGR = 0x80310006,
    /// <summary>At least one secure key protector is required for this operation to be performed.</summary>
    FVE_E_SECURE_KEY_REQUIRED = 0x80310007,
    /// <summary>BitLocker Drive Encryption is not enabled on this drive. Turn on BitLocker.</summary>
    FVE_E_NOT_ACTIVATED = 0x80310008,
    /// <summary>BitLocker Drive Encryption cannot perform the requested action. This condition may occur when two requests are issued at the same time. Wait a few moments and then try the action again.</summary>
    FVE_E_ACTION_NOT_ALLOWED = 0x80310009,
    /// <summary>The Active Directory Domain Services forest does not contain the required attributes and classes to host BitLocker Drive Encryption or Trusted Platform Module information. Contact your domain administrator to verify that any required BitLocker Active Directory schema extensions have been installed.</summary>
    FVE_E_AD_SCHEMA_NOT_INSTALLED = 0x8031000A,
    /// <summary>The type of the data obtained from Active Directory was not expected. The BitLocker recovery information may be missing or corrupted.</summary>
    FVE_E_AD_INVALID_DATATYPE = 0x8031000B,
    /// <summary>The size of the data obtained from Active Directory was not expected. The BitLocker recovery information may be missing or corrupted.</summary>
    FVE_E_AD_INVALID_DATASIZE = 0x8031000C,
    /// <summary>The attribute read from Active Directory does not contain any values. The BitLocker recovery information may be missing or corrupted.</summary>
    FVE_E_AD_NO_VALUES = 0x8031000D,
    /// <summary>The attribute was not set. Verify that you are logged on with a domain account that has the ability to write information to Active Directory objects.</summary>
    FVE_E_AD_ATTR_NOT_SET = 0x8031000E,
    /// <summary>The specified attribute cannot be found in Active Directory Domain Services. Contact your domain administrator to verify that any required BitLocker Active Directory schema extensions have been installed.</summary>
    FVE_E_AD_GUID_NOT_FOUND = 0x8031000F,
    /// <summary>The BitLocker metadata for the encrypted drive is not valid. You can attempt to repair the drive to restore access.</summary>
    FVE_E_BAD_INFORMATION = 0x80310010,
    /// <summary>The drive cannot be encrypted because it does not have enough free space. Delete any unnecessary data on the drive to create additional free space and then try again.</summary>
    FVE_E_TOO_SMALL = 0x80310011,
    /// <summary>The drive cannot be encrypted because it contains system boot information. Create a separate partition for use as the system drive that contains the boot information and a second partition for use as the operating system drive and then encrypt the operating system drive.</summary>
    FVE_E_SYSTEM_VOLUME = 0x80310012,
    /// <summary>The drive cannot be encrypted because the file system is not supported.</summary>
    FVE_E_FAILED_WRONG_FS = 0x80310013,
    /// <summary>The file system size is larger than the partition size in the partition table. This drive may be corrupt or may have been tampered with. To use it with BitLocker, you must reformat the partition.</summary>
    FVE_E_BAD_PARTITION_SIZE = 0x80310014,
    /// <summary>This drive cannot be encrypted.</summary>
    FVE_E_NOT_SUPPORTED = 0x80310015,
    /// <summary>The data is not valid.</summary>
    FVE_E_BAD_DATA = 0x80310016,
    /// <summary>The data drive specified is not set to automatically unlock on the current computer and cannot be unlocked automatically.</summary>
    FVE_E_VOLUME_NOT_BOUND = 0x80310017,
    /// <summary>You must initialize the Trusted Platform Module (TPM) before you can use BitLocker Drive Encryption.</summary>
    FVE_E_TPM_NOT_OWNED = 0x80310018,
    /// <summary>The operation attempted cannot be performed on an operating system drive.</summary>
    FVE_E_NOT_DATA_VOLUME = 0x80310019,
    /// <summary>The buffer supplied to a function was insufficient to contain the returned data. Increase the buffer size before running the function again.</summary>
    FVE_E_AD_INSUFFICIENT_BUFFER = 0x8031001A,
    /// <summary>A read operation failed while converting the drive. The drive was not converted. Please re-enable BitLocker.</summary>
    FVE_E_CONV_READ = 0x8031001B,
    /// <summary>A write operation failed while converting the drive. The drive was not converted. Please re-enable BitLocker.</summary>
    FVE_E_CONV_WRITE = 0x8031001C,
    /// <summary>One or more BitLocker key protectors are required. You cannot delete the last key on this drive.</summary>
    FVE_E_KEY_REQUIRED = 0x8031001D,
    /// <summary>Cluster configurations are not supported by BitLocker Drive Encryption.</summary>
    FVE_E_CLUSTERING_NOT_SUPPORTED = 0x8031001E,
    /// <summary>The drive specified is already configured to be automatically unlocked on the current computer.</summary>
    FVE_E_VOLUME_BOUND_ALREADY = 0x8031001F,
    /// <summary>The operating system drive is not protected by BitLocker Drive Encryption.</summary>
    FVE_E_OS_NOT_PROTECTED = 0x80310020,
    /// <summary>BitLocker Drive Encryption has been suspended on this drive. All BitLocker key protectors configured for this drive are effectively disabled, and the drive will be automatically unlocked using an unencrypted (clear) key.</summary>
    FVE_E_PROTECTION_DISABLED = 0x80310021,
    /// <summary>The drive you are attempting to lock does not have any key protectors available for encryption because BitLocker protection is currently suspended. Re-enable BitLocker to lock this drive.</summary>
    FVE_E_RECOVERY_KEY_REQUIRED = 0x80310022,
    /// <summary>BitLocker cannot use the Trusted Platform Module (TPM) to protect a data drive. TPM protection can only be used with the operating system drive.</summary>
    FVE_E_FOREIGN_VOLUME = 0x80310023,
    /// <summary>The BitLocker metadata for the encrypted drive cannot be updated because it was locked for updating by another process. Please try this process again.</summary>
    FVE_E_OVERLAPPED_UPDATE = 0x80310024,
    /// <summary>The authorization data for the storage root key (SRK) of the Trusted Platform Module (TPM) is not zero and is therefore incompatible with BitLocker. Please initialize the TPM before attempting to use it with BitLocker.</summary>
    FVE_E_TPM_SRK_AUTH_NOT_ZERO = 0x80310025,
    /// <summary>The drive encryption algorithm cannot be used on this sector size.</summary>
    FVE_E_FAILED_SECTOR_SIZE = 0x80310026,
    /// <summary>The drive cannot be unlocked with the key provided. Confirm that you have provided the correct key and try again.</summary>
    FVE_E_FAILED_AUTHENTICATION = 0x80310027,
    /// <summary>The drive specified is not the operating system drive.</summary>
    FVE_E_NOT_OS_VOLUME = 0x80310028,
    /// <summary>BitLocker Drive Encryption cannot be turned off on the operating system drive until the auto unlock feature has been disabled for the fixed data drives and removable data drives associated with this computer.</summary>
    FVE_E_AUTOUNLOCK_ENABLED = 0x80310029,
    /// <summary>The system partition boot sector does not perform Trusted Platform Module (TPM) measurements. Use the Bootrec.exe tool in the Windows Recovery Environment to update or repair the boot sector.</summary>
    FVE_E_WRONG_BOOTSECTOR = 0x8031002A,
    /// <summary>BitLocker Drive Encryption operating system drives must be formatted with the NTFS file system in order to be encrypted. Convert the drive to NTFS, and then turn on BitLocker.</summary>
    FVE_E_WRONG_SYSTEM_FS = 0x8031002B,
    /// <summary>Group Policy settings require that a recovery password be specified before encrypting the drive.</summary>
    FVE_E_POLICY_PASSWORD_REQUIRED = 0x8031002C,
    /// <summary>The drive encryption algorithm and key cannot be set on a previously encrypted drive. To encrypt this drive with BitLocker Drive Encryption, remove the previous encryption and then turn on BitLocker.</summary>
    FVE_E_CANNOT_SET_FVEK_ENCRYPTED = 0x8031002D,
    /// <summary>BitLocker Drive Encryption cannot encrypt the specified drive because an encryption key is not available. Add a key protector to encrypt this drive.</summary>
    FVE_E_CANNOT_ENCRYPT_NO_KEY = 0x8031002E,
    /// <summary>BitLocker Drive Encryption detected bootable media (CD or DVD) in the computer. Remove the media and restart the computer before configuring BitLocker.</summary>
    FVE_E_BOOTABLE_CDDVD = 0x80310030,
    /// <summary>This key protector cannot be added. Only one key protector of this type is allowed for this drive.</summary>
    FVE_E_PROTECTOR_EXISTS = 0x80310031,
    /// <summary>The recovery password file was not found because a relative path was specified. Recovery passwords must be saved to a fully qualified path. Environment variables configured on the computer can be used in the path.</summary>
    FVE_E_RELATIVE_PATH = 0x80310032,
    /// <summary>The specified key protector was not found on the drive. Try another key protector.</summary>
    FVE_E_PROTECTOR_NOT_FOUND = 0x80310033,
    /// <summary>The recovery key provided is corrupt and cannot be used to access the drive. An alternative recovery method, such as recovery password, a data recovery agent, or a backup version of the recovery key must be used to recover access to the drive.</summary>
    FVE_E_INVALID_KEY_FORMAT = 0x80310034,
    /// <summary>The format of the recovery password provided is invalid. BitLocker recovery passwords are 48 digits. Verify that the recovery password is in the correct format and then try again.</summary>
    FVE_E_INVALID_PASSWORD_FORMAT = 0x80310035,
    /// <summary>The random number generator check test failed.</summary>
    FVE_E_FIPS_RNG_CHECK_FAILED = 0x80310036,
    /// <summary>The Group Policy setting requiring FIPS compliance prevents a local recovery password from being generated or used by BitLocker Drive Encryption. When operating in FIPS-compliant mode, BitLocker recovery options can be either a recovery key stored on a USB drive or recovery through a data recovery agent.</summary>
    FVE_E_FIPS_PREVENTS_RECOVERY_PASSWORD = 0x80310037,
    /// <summary>The Group Policy setting requiring FIPS compliance prevents the recovery password from being saved to Active Directory. When operating in FIPS-compliant mode, BitLocker recovery options can be either a recovery key stored on a USB drive or recovery through a data recovery agent. Check your Group Policy settings configuration.</summary>
    FVE_E_FIPS_PREVENTS_EXTERNAL_KEY_EXPORT = 0x80310038,
    /// <summary>The drive must be fully decrypted to complete this operation.</summary>
    FVE_E_NOT_DECRYPTED = 0x80310039,
    /// <summary>The key protector specified cannot be used for this operation.</summary>
    FVE_E_INVALID_PROTECTOR_TYPE = 0x8031003A,
    /// <summary>No key protectors exist on the drive to perform the hardware test.</summary>
    FVE_E_NO_PROTECTORS_TO_TEST = 0x8031003B,
    /// <summary>The BitLocker startup key or recovery password cannot be found on the USB device. Verify that you have the correct USB device, that the USB device is plugged into the computer on an active USB port, restart the computer, and then try again. If the problem persists, contact the computer manufacturer for BIOS upgrade instructions.</summary>
    FVE_E_KEYFILE_NOT_FOUND = 0x8031003C,
    /// <summary>The BitLocker startup key or recovery password file provided is corrupt or invalid. Verify that you have the correct startup key or recovery password file and try again.</summary>
    FVE_E_KEYFILE_INVALID = 0x8031003D,
    /// <summary>The BitLocker encryption key cannot be obtained from the startup key or recovery password. Verify that you have the correct startup key or recovery password and try again.</summary>
    FVE_E_KEYFILE_NO_VMK = 0x8031003E,
    /// <summary>The Trusted Platform Module (TPM) is disabled. The TPM must be enabled, initialized, and have valid ownership before it can be used with BitLocker Drive Encryption.</summary>
    FVE_E_TPM_DISABLED = 0x8031003F,
    /// <summary>The BitLocker configuration of the specified drive cannot be managed because this computer is currently operating in Safe Mode. While in Safe Mode, BitLocker Drive Encryption can only be used for recovery purposes.</summary>
    FVE_E_NOT_ALLOWED_IN_SAFE_MODE = 0x80310040,
    /// <summary>The Trusted Platform Module (TPM) was not able to unlock the drive because the system boot information has changed or a PIN was not provided correctly. Verify that the drive has not been tampered with and that changes to the system boot information were caused by a trusted source. After verifying that the drive is safe to access, use the BitLocker recovery console to unlock the drive and then suspend and resume BitLocker to update system boot information that BitLocker associates with this drive.</summary>
    FVE_E_TPM_INVALID_PCR = 0x80310041,
    /// <summary>The BitLocker encryption key cannot be obtained from the Trusted Platform Module (TPM).</summary>
    FVE_E_TPM_NO_VMK = 0x80310042,
    /// <summary>The BitLocker encryption key cannot be obtained from the Trusted Platform Module (TPM) and PIN.</summary>
    FVE_E_PIN_INVALID = 0x80310043,
    /// <summary>A boot application has changed since BitLocker Drive Encryption was enabled.</summary>
    FVE_E_AUTH_INVALID_APPLICATION = 0x80310044,
    /// <summary>The Boot Configuration Data (BCD) settings have changed since BitLocker Drive Encryption was enabled.</summary>
    FVE_E_AUTH_INVALID_CONFIG = 0x80310045,
    /// <summary>The Group Policy setting requiring FIPS compliance prohibits the use of unencrypted keys, which prevents BitLocker from being suspended on this drive. Please contact your domain administrator for more information.</summary>
    FVE_E_FIPS_DISABLE_PROTECTION_NOT_ALLOWED = 0x80310046,
    /// <summary>This drive cannot be encrypted by BitLocker Drive Encryption because the file system does not extend to the end of the drive. Repartition this drive and then try again.</summary>
    FVE_E_FS_NOT_EXTENDED = 0x80310047,
    /// <summary>BitLocker Drive Encryption cannot be enabled on the operating system drive. Contact the computer manufacturer for BIOS upgrade instructions.</summary>
    FVE_E_FIRMWARE_TYPE_NOT_SUPPORTED = 0x80310048,
    /// <summary>This version of Windows does not include BitLocker Drive Encryption. To use BitLocker Drive Encryption, please upgrade the operating system.</summary>
    FVE_E_NO_LICENSE = 0x80310049,
    /// <summary>BitLocker Drive Encryption cannot be used because critical BitLocker system files are missing or corrupted. Use Windows Startup Repair to restore these files to your computer.</summary>
    FVE_E_NOT_ON_STACK = 0x8031004A,
    /// <summary>The drive cannot be locked when the drive is in use.</summary>
    FVE_E_FS_MOUNTED = 0x8031004B,
    /// <summary>The access token associated with the current thread is not an impersonated token.</summary>
    FVE_E_TOKEN_NOT_IMPERSONATED = 0x8031004C,
    /// <summary>The BitLocker encryption key cannot be obtained. Verify that the Trusted Platform Module (TPM) is enabled and ownership has been taken. If this computer does not have a TPM, verify that the USB drive is inserted and available.</summary>
    FVE_E_DRY_RUN_FAILED = 0x8031004D,
    /// <summary>You must restart your computer before continuing with BitLocker Drive Encryption.</summary>
    FVE_E_REBOOT_REQUIRED = 0x8031004E,
    /// <summary>Drive encryption cannot occur while boot debugging is enabled. Use the bcdedit command-line tool to turn off boot debugging.</summary>
    FVE_E_DEBUGGER_ENABLED = 0x8031004F,
    /// <summary>No action was taken as BitLocker Drive Encryption is in raw access mode.</summary>
    FVE_E_RAW_ACCESS = 0x80310050,
    /// <summary>BitLocker Drive Encryption cannot enter raw access mode for this drive because the drive is currently in use.</summary>
    FVE_E_RAW_BLOCKED = 0x80310051,
    /// <summary>The path specified in the Boot Configuration Data (BCD) for a BitLocker Drive Encryption integrity-protected application is incorrect. Please verify and correct your BCD settings and try again.</summary>
    FVE_E_BCD_APPLICATIONS_PATH_INCORRECT = 0x80310052,
    /// <summary>BitLocker Drive Encryption can only be used for recovery purposes when the computer is running Windows Recovery Environment.</summary>
    FVE_E_NOT_ALLOWED_IN_VERSION = 0x80310053,
    /// <summary>The auto-unlock master key was not available from the operating system drive.</summary>
    FVE_E_NO_AUTOUNLOCK_MASTER_KEY = 0x80310054,
    /// <summary>The system firmware failed to enable clearing of system memory when the computer was restarted.</summary>
    FVE_E_MOR_FAILED = 0x80310055,
    /// <summary>The hidden drive cannot be encrypted.</summary>
    FVE_E_HIDDEN_VOLUME = 0x80310056,
    /// <summary>BitLocker encryption keys were ignored because the drive was in a transient state.</summary>
    FVE_E_TRANSIENT_STATE = 0x80310057,
    /// <summary>Public key based protectors are not allowed on this drive.</summary>
    FVE_E_PUBKEY_NOT_ALLOWED = 0x80310058,
    /// <summary>BitLocker Drive Encryption is already performing an operation on this drive. Please complete all operations before continuing.</summary>
    FVE_E_VOLUME_HANDLE_OPEN = 0x80310059,
    /// <summary>This version of Windows does not support this feature of BitLocker Drive Encryption. To use this feature, upgrade the operating system..</summary>
    FVE_E_NO_FEATURE_LICENSE = 0x8031005A,
    /// <summary>The Group Policy settings for BitLocker startup options are in conflict and cannot be applied. Contact your system administrator for more information.</summary>
    FVE_E_INVALID_STARTUP_OPTIONS = 0x8031005B,
    /// <summary>Group Policy settings do not permit the creation of a recovery password.</summary>
    FVE_E_POLICY_RECOVERY_PASSWORD_NOT_ALLOWED = 0x8031005C,
    /// <summary>Group Policy settings require the creation of a recovery password.</summary>
    FVE_E_POLICY_RECOVERY_PASSWORD_REQUIRED = 0x8031005D,
    /// <summary>Group Policy settings do not permit the creation of a recovery key.</summary>
    FVE_E_POLICY_RECOVERY_KEY_NOT_ALLOWED = 0x8031005E,
    /// <summary>Group Policy settings require the creation of a recovery key.</summary>
    FVE_E_POLICY_RECOVERY_KEY_REQUIRED = 0x8031005F,
    /// <summary>Group Policy settings do not permit the use of a PIN at startup. Please choose a different BitLocker startup option.</summary>
    FVE_E_POLICY_STARTUP_PIN_NOT_ALLOWED = 0x80310060,
    /// <summary>Group Policy settings require the use of a PIN at startup. Please choose this BitLocker startup option.</summary>
    FVE_E_POLICY_STARTUP_PIN_REQUIRED = 0x80310061,
    /// <summary>Group Policy settings do not permit the use of a startup key. Please choose a different BitLocker startup option.</summary>
    FVE_E_POLICY_STARTUP_KEY_NOT_ALLOWED = 0x80310062,
    /// <summary>Group Policy settings require the use of a startup key. Please choose this BitLocker startup option.</summary>
    FVE_E_POLICY_STARTUP_KEY_REQUIRED = 0x80310063,
    /// <summary>Group Policy settings do not permit the use of a startup key and PIN. Please choose a different BitLocker startup option.</summary>
    FVE_E_POLICY_STARTUP_PIN_KEY_NOT_ALLOWED = 0x80310064,
    /// <summary>Group Policy settings require the use of a startup key and PIN. Please choose this BitLocker startup option.</summary>
    FVE_E_POLICY_STARTUP_PIN_KEY_REQUIRED = 0x80310065,
    /// <summary>Group policy does not permit the use of TPM-only at startup. Please choose a different BitLocker startup option.</summary>
    FVE_E_POLICY_STARTUP_TPM_NOT_ALLOWED = 0x80310066,
    /// <summary>Group Policy settings require the use of TPM-only at startup. Please choose this BitLocker startup option.</summary>
    FVE_E_POLICY_STARTUP_TPM_REQUIRED = 0x80310067,
    /// <summary>The PIN provided does not meet minimum or maximum length requirements.</summary>
    FVE_E_POLICY_INVALID_PIN_LENGTH = 0x80310068,
    /// <summary>The key protector is not supported by the version of BitLocker Drive Encryption currently on the drive. Upgrade the drive to add the key protector.</summary>
    FVE_E_KEY_PROTECTOR_NOT_SUPPORTED = 0x80310069,
    /// <summary>Group Policy settings do not permit the creation of a password.</summary>
    FVE_E_POLICY_PASSPHRASE_NOT_ALLOWED = 0x8031006A,
    /// <summary>Group Policy settings require the creation of a password.</summary>
    FVE_E_POLICY_PASSPHRASE_REQUIRED = 0x8031006B,
    /// <summary>The Group Policy setting requiring FIPS compliance prevents passwords from being generated or used. Please contact your system administrator for more information.</summary>
    FVE_E_FIPS_PREVENTS_PASSPHRASE = 0x8031006C,
    /// <summary>A password cannot be added to the operating system drive.</summary>
    FVE_E_OS_VOLUME_PASSPHRASE_NOT_ALLOWED = 0x8031006D,
    /// <summary>The BitLocker object identifier (OID) on the drive appears to be invalid or corrupt. Use manage-BDE to reset the OID on this drive.</summary>
    FVE_E_INVALID_BITLOCKER_OID = 0x8031006E,
    /// <summary>The drive is too small to be protected using BitLocker Drive Encryption.</summary>
    FVE_E_VOLUME_TOO_SMALL = 0x8031006F,
    /// <summary>The selected discovery drive type is incompatible with the file system on the drive. BitLocker To Go discovery drives must be created on FAT formatted drives.</summary>
    FVE_E_DV_NOT_SUPPORTED_ON_FS = 0x80310070,
    /// <summary>The selected discovery drive type is not allowed by the computer&#39;s Group Policy settings. Verify that Group Policy settings allow the creation of discovery drives for use with BitLocker To Go.</summary>
    FVE_E_DV_NOT_ALLOWED_BY_GP = 0x80310071,
    /// <summary>Group Policy settings do not permit user certificates such as smart cards to be used with BitLocker Drive Encryption.</summary>
    FVE_E_POLICY_USER_CERTIFICATE_NOT_ALLOWED = 0x80310072,
    /// <summary>Group Policy settings require that you have a valid user certificate, such as a smart card, to be used with BitLocker Drive Encryption.</summary>
    FVE_E_POLICY_USER_CERTIFICATE_REQUIRED = 0x80310073,
    /// <summary>Group Policy settings requires that you use a smart card-based key protector with BitLocker Drive Encryption.</summary>
    FVE_E_POLICY_USER_CERT_MUST_BE_HW = 0x80310074,
    /// <summary>Group Policy settings do not permit BitLocker-protected fixed data drives to be automatically unlocked.</summary>
    FVE_E_POLICY_USER_CONFIGURE_FDV_AUTOUNLOCK_NOT_ALLOWED = 0x80310075,
    /// <summary>Group Policy settings do not permit BitLocker-protected removable data drives to be automatically unlocked.</summary>
    FVE_E_POLICY_USER_CONFIGURE_RDV_AUTOUNLOCK_NOT_ALLOWED = 0x80310076,
    /// <summary>Group Policy settings do not permit you to configure BitLocker Drive Encryption on removable data drives.</summary>
    FVE_E_POLICY_USER_CONFIGURE_RDV_NOT_ALLOWED = 0x80310077,
    /// <summary>Group Policy settings do not permit you to turn on BitLocker Drive Encryption on removable data drives. Please contact your system administrator if you need to turn on BitLocker.</summary>
    FVE_E_POLICY_USER_ENABLE_RDV_NOT_ALLOWED = 0x80310078,
    /// <summary>Group Policy settings do not permit turning off BitLocker Drive Encryption on removable data drives. Please contact your system administrator if you need to turn off BitLocker.</summary>
    FVE_E_POLICY_USER_DISABLE_RDV_NOT_ALLOWED = 0x80310079,
    /// <summary>Your password does not meet minimum password length requirements. By default, passwords must be at least 8 characters in length. Check with your system administrator for the password length requirement in your organization.</summary>
    FVE_E_POLICY_INVALID_PASSPHRASE_LENGTH = 0x80310080,
    /// <summary>Your password does not meet the complexity requirements set by your system administrator. Try adding upper and lowercase characters, numbers, and symbols.</summary>
    FVE_E_POLICY_PASSPHRASE_TOO_SIMPLE = 0x80310081,
    /// <summary>This drive cannot be encrypted because it is reserved for Windows System Recovery Options.</summary>
    FVE_E_RECOVERY_PARTITION = 0x80310082,
    /// <summary>BitLocker Drive Encryption cannot be applied to this drive because of conflicting Group Policy settings. BitLocker cannot be configured to automatically unlock fixed data drives when user recovery options are disabled. If you want BitLocker-protected fixed data drives to be automatically unlocked after key validation has occurred, please ask your system administrator to resolve the settings conflict before enabling BitLocker.</summary>
    FVE_E_POLICY_CONFLICT_FDV_RK_OFF_AUK_ON = 0x80310083,
    /// <summary>BitLocker Drive Encryption cannot be applied to this drive because of conflicting Group Policy settings. BitLocker cannot be configured to automatically unlock removable data drives when user recovery option are disabled. If you want BitLocker-protected removable data drives to be automatically unlocked after key validation has occured, please ask your system administrator to resolve the settings conflict before enabling BitLocker.</summary>
    FVE_E_POLICY_CONFLICT_RDV_RK_OFF_AUK_ON = 0x80310084,
    /// <summary>The Enhanced Key Usage (EKU) attribute of the specified certificate does not permit it to be used for BitLocker Drive Encryption. BitLocker does not require that a certificate have an EKU attribute, but if one is configured it must be set to an object identifier (OID) that matches the OID configured for BitLocker.</summary>
    FVE_E_NON_BITLOCKER_OID = 0x80310085,
    /// <summary>BitLocker Drive Encryption cannot be applied to this drive as currently configured because of Group Policy settings. The certificate you provided for drive encryption is self-signed. Current Group Policy settings do not permit the use of self-signed certificates. Obtain a new certificate from your certification authority before attempting to enable BitLocker.</summary>
    FVE_E_POLICY_PROHIBITS_SELFSIGNED = 0x80310086,
    /// <summary>BitLocker Encryption cannot be applied to this drive because of conflicting Group Policy settings. When write access to drives not protected by BitLocker is denied, the use of a USB startup key cannot be required. Please have your system administrator resolve these policy conflicts before attempting to enable BitLocker.</summary>
    FVE_E_POLICY_CONFLICT_RO_AND_STARTUP_KEY_REQUIRED = 0x80310087,
    /// <summary>BitLocker Drive Encryption failed to recover from an abruptly terminated conversion. This could be due to either all conversion logs being corrupted or the media being write-protected.</summary>
    FVE_E_CONV_RECOVERY_FAILED = 0x80310088,
    /// <summary>The requested virtualization size is too big.</summary>
    FVE_E_VIRTUALIZED_SPACE_TOO_BIG = 0x80310089,
    /// <summary>BitLocker Drive Encryption cannot be applied to this drive because there are conflicting Group Policy settings for recovery options on operating system drives. Storing recovery information to Active Directory Domain Services cannot be required when the generation of recovery passwords is not permitted. Please have your system administrator resolve these policy conflicts before attempting to enable BitLocker.</summary>
    FVE_E_POLICY_CONFLICT_OSV_RP_OFF_ADB_ON = 0x80310090,
    /// <summary>BitLocker Drive Encryption cannot be applied to this drive because there are conflicting Group Policy settings for recovery options on fixed data drives. Storing recovery information to Active Directory Domain Services cannot be required when the generation of recovery passwords is not permitted. Please have your system administrator resolve these policy conflicts before attempting to enable BitLocker.</summary>
    FVE_E_POLICY_CONFLICT_FDV_RP_OFF_ADB_ON = 0x80310091,
    /// <summary>BitLocker Drive Encryption cannot be applied to this drive because there are conflicting Group Policy settings for recovery options on removable data drives. Storing recovery information to Active Directory Domain Services cannot be required when the generation of recovery passwords is not permitted. Please have your system administrator resolve these policy conflicts before attempting to enable BitLocker.</summary>
    FVE_E_POLICY_CONFLICT_RDV_RP_OFF_ADB_ON = 0x80310092,
    /// <summary>The Key Usage (KU) attribute of the specified certificate does not permit it to be used for BitLocker Drive Encryption. BitLocker does not require that a certificate have a KU attribute, but if one is configured it must be set to either Key Encipherment or Key Agreement.</summary>
    FVE_E_NON_BITLOCKER_KU = 0x80310093,
    /// <summary>The private key associated with the specified certificate cannot be authorized. The private key authorization was either not provided or the provided authorization was invalid.</summary>
    FVE_E_PRIVATEKEY_AUTH_FAILED = 0x80310094,
    /// <summary>Removal of the data recovery agent certificate must be done using the Certificates snap-in.</summary>
    FVE_E_REMOVAL_OF_DRA_FAILED = 0x80310095,
    /// <summary>This drive was encrypted using the version of BitLocker Drive Encryption included with Windows Vista and Windows Server 2008 which does not support organizational identifiers. To specify organizational identifiers for this drive upgrade the drive encryption to the latest version using the &quot;manage-bde -upgrade&quot; command.</summary>
    FVE_E_OPERATION_NOT_SUPPORTED_ON_VISTA_VOLUME = 0x80310096,
    /// <summary>The drive cannot be locked because it is automatically unlocked on this computer.  Remove the automatic unlock protector to lock this drive.</summary>
    FVE_E_CANT_LOCK_AUTOUNLOCK_ENABLED_VOLUME = 0x80310097,
    /// <summary>The default BitLocker Key Derivation Function SP800-56A for ECC smart cards is not supported by your smart card. The Group Policy setting requiring FIPS-compliance prevents BitLocker from using any other key derivation function for encryption. You have to use a FIPS compliant smart card in FIPS restricted environments.</summary>
    FVE_E_FIPS_HASH_KDF_NOT_ALLOWED = 0x80310098,
    /// <summary>The BitLocker encryption key could not be obtained from the Trusted Platform Module (TPM) and enhanced PIN. Try using a PIN containing only numerals.</summary>
    FVE_E_ENH_PIN_INVALID = 0x80310099,
    /// <summary>The requested TPM PIN contains invalid characters.</summary>
    FVE_E_INVALID_PIN_CHARS = 0x8031009A,
    /// <summary>The management information stored on the drive contained an unknown type. If you are using an old version of Windows, try accessing the drive from the latest version.</summary>
    FVE_E_INVALID_DATUM_TYPE = 0x8031009B,
    /// <summary>The callout does not exist.</summary>
    FWP_E_CALLOUT_NOT_FOUND = 0x80320001,
    /// <summary>The filter condition does not exist.</summary>
    FWP_E_CONDITION_NOT_FOUND = 0x80320002,
    /// <summary>The filter does not exist.</summary>
    FWP_E_FILTER_NOT_FOUND = 0x80320003,
    /// <summary>The layer does not exist.</summary>
    FWP_E_LAYER_NOT_FOUND = 0x80320004,
    /// <summary>The provider does not exist.</summary>
    FWP_E_PROVIDER_NOT_FOUND = 0x80320005,
    /// <summary>The provider context does not exist.</summary>
    FWP_E_PROVIDER_CONTEXT_NOT_FOUND = 0x80320006,
    /// <summary>The sublayer does not exist.</summary>
    FWP_E_SUBLAYER_NOT_FOUND = 0x80320007,
    /// <summary>The object does not exist.</summary>
    FWP_E_NOT_FOUND = 0x80320008,
    /// <summary>An object with that GUID or LUID already exists.</summary>
    FWP_E_ALREADY_EXISTS = 0x80320009,
    /// <summary>The object is referenced by other objects so cannot be deleted.</summary>
    FWP_E_IN_USE = 0x8032000A,
    /// <summary>The call is not allowed from within a dynamic session.</summary>
    FWP_E_DYNAMIC_SESSION_IN_PROGRESS = 0x8032000B,
    /// <summary>The call was made from the wrong session so cannot be completed.</summary>
    FWP_E_WRONG_SESSION = 0x8032000C,
    /// <summary>The call must be made from within an explicit transaction.</summary>
    FWP_E_NO_TXN_IN_PROGRESS = 0x8032000D,
    /// <summary>The call is not allowed from within an explicit transaction.</summary>
    FWP_E_TXN_IN_PROGRESS = 0x8032000E,
    /// <summary>The explicit transaction has been forcibly cancelled.</summary>
    FWP_E_TXN_ABORTED = 0x8032000F,
    /// <summary>The session has been cancelled.</summary>
    FWP_E_SESSION_ABORTED = 0x80320010,
    /// <summary>The call is not allowed from within a read-only transaction.</summary>
    FWP_E_INCOMPATIBLE_TXN = 0x80320011,
    /// <summary>The call timed out while waiting to acquire the transaction lock.</summary>
    FWP_E_TIMEOUT = 0x80320012,
    /// <summary>Collection of network diagnostic events is disabled.</summary>
    FWP_E_NET_EVENTS_DISABLED = 0x80320013,
    /// <summary>The operation is not supported by the specified layer.</summary>
    FWP_E_INCOMPATIBLE_LAYER = 0x80320014,
    /// <summary>The call is allowed for kernel-mode callers only.</summary>
    FWP_E_KM_CLIENTS_ONLY = 0x80320015,
    /// <summary>The call tried to associate two objects with incompatible lifetimes.</summary>
    FWP_E_LIFETIME_MISMATCH = 0x80320016,
    /// <summary>The object is built in so cannot be deleted.</summary>
    FWP_E_BUILTIN_OBJECT = 0x80320017,
    /// <summary>The maximum number of callouts has been reached.</summary>
    FWP_E_TOO_MANY_CALLOUTS = 0x80320018,
    /// <summary>A notification could not be delivered because a message queue is at its maximum capacity.</summary>
    FWP_E_NOTIFICATION_DROPPED = 0x80320019,
    /// <summary>The traffic parameters do not match those for the security association context.</summary>
    FWP_E_TRAFFIC_MISMATCH = 0x8032001A,
    /// <summary>The call is not allowed for the current security association state.</summary>
    FWP_E_INCOMPATIBLE_SA_STATE = 0x8032001B,
    /// <summary>A required pointer is null.</summary>
    FWP_E_NULL_POINTER = 0x8032001C,
    /// <summary>An enumerator is not valid.</summary>
    FWP_E_INVALID_ENUMERATOR = 0x8032001D,
    /// <summary>The flags field contains an invalid value.</summary>
    FWP_E_INVALID_FLAGS = 0x8032001E,
    /// <summary>A network mask is not valid.</summary>
    FWP_E_INVALID_NET_MASK = 0x8032001F,
    /// <summary>An FWP_RANGE is not valid.</summary>
    FWP_E_INVALID_RANGE = 0x80320020,
    /// <summary>The time interval is not valid.</summary>
    FWP_E_INVALID_INTERVAL = 0x80320021,
    /// <summary>An array that must contain at least one element is zero length.</summary>
    FWP_E_ZERO_LENGTH_ARRAY = 0x80320022,
    /// <summary>The displayData.name field cannot be null.</summary>
    FWP_E_NULL_DISPLAY_NAME = 0x80320023,
    /// <summary>The action type is not one of the allowed action types for a filter.</summary>
    FWP_E_INVALID_ACTION_TYPE = 0x80320024,
    /// <summary>The filter weight is not valid.</summary>
    FWP_E_INVALID_WEIGHT = 0x80320025,
    /// <summary>A filter condition contains a match type that is not compatible with the operands.</summary>
    FWP_E_MATCH_TYPE_MISMATCH = 0x80320026,
    /// <summary>An FWP_VALUE or FWPM_CONDITION_VALUE is of the wrong type.</summary>
    FWP_E_TYPE_MISMATCH = 0x80320027,
    /// <summary>An integer value is outside the allowed range.</summary>
    FWP_E_OUT_OF_BOUNDS = 0x80320028,
    /// <summary>A reserved field is non-zero.</summary>
    FWP_E_RESERVED = 0x80320029,
    /// <summary>A filter cannot contain multiple conditions operating on a single field.</summary>
    FWP_E_DUPLICATE_CONDITION = 0x8032002A,
    /// <summary>A policy cannot contain the same keying module more than once.</summary>
    FWP_E_DUPLICATE_KEYMOD = 0x8032002B,
    /// <summary>The action type is not compatible with the layer.</summary>
    FWP_E_ACTION_INCOMPATIBLE_WITH_LAYER = 0x8032002C,
    /// <summary>The action type is not compatible with the sublayer.</summary>
    FWP_E_ACTION_INCOMPATIBLE_WITH_SUBLAYER = 0x8032002D,
    /// <summary>The raw context or the provider context is not compatible with the layer.</summary>
    FWP_E_CONTEXT_INCOMPATIBLE_WITH_LAYER = 0x8032002E,
    /// <summary>The raw context or the provider context is not compatible with the callout.</summary>
    FWP_E_CONTEXT_INCOMPATIBLE_WITH_CALLOUT = 0x8032002F,
    /// <summary>The authentication method is not compatible with the policy type.</summary>
    FWP_E_INCOMPATIBLE_AUTH_METHOD = 0x80320030,
    /// <summary>The Diffie-Hellman group is not compatible with the policy type.</summary>
    FWP_E_INCOMPATIBLE_DH_GROUP = 0x80320031,
    /// <summary>An IKE policy cannot contain an Extended Mode policy.</summary>
    FWP_E_EM_NOT_SUPPORTED = 0x80320032,
    /// <summary>The enumeration template or subscription will never match any objects.</summary>
    FWP_E_NEVER_MATCH = 0x80320033,
    /// <summary>The provider context is of the wrong type.</summary>
    FWP_E_PROVIDER_CONTEXT_MISMATCH = 0x80320034,
    /// <summary>The parameter is incorrect.</summary>
    FWP_E_INVALID_PARAMETER = 0x80320035,
    /// <summary>The maximum number of sublayers has been reached.</summary>
    FWP_E_TOO_MANY_SUBLAYERS = 0x80320036,
    /// <summary>The notification function for a callout returned an error.</summary>
    FWP_E_CALLOUT_NOTIFICATION_FAILED = 0x80320037,
    /// <summary>The IPsec authentication transform is not valid.</summary>
    FWP_E_INVALID_AUTH_TRANSFORM = 0x80320038,
    /// <summary>The IPsec cipher transform is not valid.</summary>
    FWP_E_INVALID_CIPHER_TRANSFORM = 0x80320039,
    /// <summary>The packet should be dropped, no ICMP should be sent.</summary>
    FWP_E_DROP_NOICMP = 0x80320104,
    /// <summary>The IPsec cipher transform is not compatible with the policy.</summary>
    FWP_E_INCOMPATIBLE_CIPHER_TRANSFORM = 0x8032003A,
    /// <summary>The combination of IPsec transform types is not valid.</summary>
    FWP_E_INVALID_TRANSFORM_COMBINATION = 0x8032003B,
    /// <summary>A policy cannot contain the same auth method more than once.</summary>
    FWP_E_DUPLICATE_AUTH_METHOD = 0x8032003C,
    /// <summary>The function call is completing asynchronously.</summary>
    WS_S_ASYNC = 0x003D0000,
    /// <summary>There are no more messages available on the channel.</summary>
    WS_S_END = 0x003D0001,
    /// <summary>The input data was not in the expected format or did not have the expected value.</summary>
    WS_E_INVALID_FORMAT = 0x803D0000,
    /// <summary>The operation could not be completed because the object is in a faulted state due to a previous error.</summary>
    WS_E_OBJECT_FAULTED = 0x803D0001,
    /// <summary>The operation could not be completed because it would lead to numeric overflow.</summary>
    WS_E_NUMERIC_OVERFLOW = 0x803D0002,
    /// <summary>The operation is not allowed due to the current state of the object.</summary>
    WS_E_INVALID_OPERATION = 0x803D0003,
    /// <summary>The operation was aborted.</summary>
    WS_E_OPERATION_ABORTED = 0x803D0004,
    /// <summary>Access was denied by the remote endpoint.</summary>
    WS_E_ENDPOINT_ACCESS_DENIED = 0x803D0005,
    /// <summary>The operation did not complete within the time allotted.</summary>
    WS_E_OPERATION_TIMED_OUT = 0x803D0006,
    /// <summary>The operation was abandoned.</summary>
    WS_E_OPERATION_ABANDONED = 0x803D0007,
    /// <summary>A quota was exceeded.</summary>
    WS_E_QUOTA_EXCEEDED = 0x803D0008,
    /// <summary>The information was not available in the specified language.</summary>
    WS_E_NO_TRANSLATION_AVAILABLE = 0x803D0009,
    /// <summary>Security verification was not successful for the received data.</summary>
    WS_E_SECURITY_VERIFICATION_FAILURE = 0x803D000A,
    /// <summary>The address is already being used.</summary>
    WS_E_ADDRESS_IN_USE = 0x803D000B,
    /// <summary>The address is not valid for this context.</summary>
    WS_E_ADDRESS_NOT_AVAILABLE = 0x803D000C,
    /// <summary>The remote endpoint does not exist or could not be located.</summary>
    WS_E_ENDPOINT_NOT_FOUND = 0x803D000D,
    /// <summary>The remote endpoint is not currently in service at this location.</summary>
    WS_E_ENDPOINT_NOT_AVAILABLE = 0x803D000E,
    /// <summary>The remote endpoint could not process the request.</summary>
    WS_E_ENDPOINT_FAILURE = 0x803D000F,
    /// <summary>The remote endpoint was not reachable.</summary>
    WS_E_ENDPOINT_UNREACHABLE = 0x803D0010,
    /// <summary>The operation was not supported by the remote endpoint.</summary>
    WS_E_ENDPOINT_ACTION_NOT_SUPPORTED = 0x803D0011,
    /// <summary>The remote endpoint is unable to process the request due to being overloaded.</summary>
    WS_E_ENDPOINT_TOO_BUSY = 0x803D0012,
    /// <summary>A message containing a fault was received from the remote endpoint.</summary>
    WS_E_ENDPOINT_FAULT_RECEIVED = 0x803D0013,
    /// <summary>The connection with the remote endpoint was terminated.</summary>
    WS_E_ENDPOINT_DISCONNECTED = 0x803D0014,
    /// <summary>The HTTP proxy server could not process the request.</summary>
    WS_E_PROXY_FAILURE = 0x803D0015,
    /// <summary>Access was denied by the HTTP proxy server.</summary>
    WS_E_PROXY_ACCESS_DENIED = 0x803D0016,
    /// <summary>The requested feature is not available on this platform.</summary>
    WS_E_NOT_SUPPORTED = 0x803D0017,
    /// <summary>The HTTP proxy server requires HTTP authentication scheme &#39;basic&#39;.</summary>
    WS_E_PROXY_REQUIRES_BASIC_AUTH = 0x803D0018,
    /// <summary>The HTTP proxy server requires HTTP authentication scheme &#39;digest&#39;.</summary>
    WS_E_PROXY_REQUIRES_DIGEST_AUTH = 0x803D0019,
    /// <summary>The HTTP proxy server requires HTTP authentication scheme &#39;NTLM&#39;.</summary>
    WS_E_PROXY_REQUIRES_NTLM_AUTH = 0x803D001A,
    /// <summary>The HTTP proxy server requires HTTP authentication scheme &#39;negotiate&#39;.</summary>
    WS_E_PROXY_REQUIRES_NEGOTIATE_AUTH = 0x803D001B,
    /// <summary>The remote endpoint requires HTTP authentication scheme &#39;basic&#39;.</summary>
    WS_E_SERVER_REQUIRES_BASIC_AUTH = 0x803D001C,
    /// <summary>The remote endpoint requires HTTP authentication scheme &#39;digest&#39;.</summary>
    WS_E_SERVER_REQUIRES_DIGEST_AUTH = 0x803D001D,
    /// <summary>The remote endpoint requires HTTP authentication scheme &#39;NTLM&#39;.</summary>
    WS_E_SERVER_REQUIRES_NTLM_AUTH = 0x803D001E,
    /// <summary>The remote endpoint requires HTTP authentication scheme &#39;negotiate&#39;.</summary>
    WS_E_SERVER_REQUIRES_NEGOTIATE_AUTH = 0x803D001F,
    /// <summary>The endpoint address URL is invalid.</summary>
    WS_E_INVALID_ENDPOINT_URL = 0x803D0020,
    /// <summary>Unrecognized error occured in the Windows Web Services framework.</summary>
    WS_E_OTHER = 0x803D0021,
    /// <summary>A security token was rejected by the server because it has expired.</summary>
    WS_E_SECURITY_TOKEN_EXPIRED = 0x803D0022,
    /// <summary>A security operation failed in the Windows Web Services framework.</summary>
    WS_E_SECURITY_SYSTEM_FAILURE = 0x803D0023,
    /// <summary>The binding to the network interface is being closed.</summary>
    ERROR_NDIS_INTERFACE_CLOSING = 0x80340002,
    /// <summary>An invalid version was specified.</summary>
    ERROR_NDIS_BAD_VERSION = 0x80340004,
    /// <summary>An invalid characteristics table was used.</summary>
    ERROR_NDIS_BAD_CHARACTERISTICS = 0x80340005,
    /// <summary>Failed to find the network interface or network interface is not ready.</summary>
    ERROR_NDIS_ADAPTER_NOT_FOUND = 0x80340006,
    /// <summary>Failed to open the network interface.</summary>
    ERROR_NDIS_OPEN_FAILED = 0x80340007,
    /// <summary>Network interface has encountered an internal unrecoverable failure.</summary>
    ERROR_NDIS_DEVICE_FAILED = 0x80340008,
    /// <summary>The multicast list on the network interface is full.</summary>
    ERROR_NDIS_MULTICAST_FULL = 0x80340009,
    /// <summary>An attempt was made to add a duplicate multicast address to the list.</summary>
    ERROR_NDIS_MULTICAST_EXISTS = 0x8034000A,
    /// <summary>At attempt was made to remove a multicast address that was never added.</summary>
    ERROR_NDIS_MULTICAST_NOT_FOUND = 0x8034000B,
    /// <summary>Netowork interface aborted the request.</summary>
    ERROR_NDIS_REQUEST_ABORTED = 0x8034000C,
    /// <summary>Network interface can not process the request because it is being reset.</summary>
    ERROR_NDIS_RESET_IN_PROGRESS = 0x8034000D,
    /// <summary>Netword interface does not support this request.</summary>
    ERROR_NDIS_NOT_SUPPORTED = 0x803400BB,
    /// <summary>An attempt was made to send an invalid packet on a network interface.</summary>
    ERROR_NDIS_INVALID_PACKET = 0x8034000F,
    /// <summary>Network interface is not ready to complete this operation.</summary>
    ERROR_NDIS_ADAPTER_NOT_READY = 0x80340011,
    /// <summary>The length of the buffer submitted for this operation is not valid.</summary>
    ERROR_NDIS_INVALID_LENGTH = 0x80340014,
    /// <summary>The data used for this operation is not valid.</summary>
    ERROR_NDIS_INVALID_DATA = 0x80340015,
    /// <summary>The length of buffer submitted for this operation is too small.</summary>
    ERROR_NDIS_BUFFER_TOO_SHORT = 0x80340016,
    /// <summary>Network interface does not support this OID (Object Identifier)</summary>
    ERROR_NDIS_INVALID_OID = 0x80340017,
    /// <summary>The network interface has been removed.</summary>
    ERROR_NDIS_ADAPTER_REMOVED = 0x80340018,
    /// <summary>Network interface does not support this media type.</summary>
    ERROR_NDIS_UNSUPPORTED_MEDIA = 0x80340019,
    /// <summary>An attempt was made to remove a token ring group address that is in use by other components.</summary>
    ERROR_NDIS_GROUP_ADDRESS_IN_USE = 0x8034001A,
    /// <summary>An attempt was made to map a file that can not be found.</summary>
    ERROR_NDIS_FILE_NOT_FOUND = 0x8034001B,
    /// <summary>An error occured while NDIS tried to map the file.</summary>
    ERROR_NDIS_ERROR_READING_FILE = 0x8034001C,
    /// <summary>An attempt was made to map a file that is alreay mapped.</summary>
    ERROR_NDIS_ALREADY_MAPPED = 0x8034001D,
    /// <summary>An attempt to allocate a hardware resource failed because the resource is used by another component.</summary>
    ERROR_NDIS_RESOURCE_CONFLICT = 0x8034001E,
    /// <summary>The I/O operation failed because network media is disconnected or wireless access point is out of range.</summary>
    ERROR_NDIS_MEDIA_DISCONNECTED = 0x8034001F,
    /// <summary>The network address used in the request is invalid.</summary>
    ERROR_NDIS_INVALID_ADDRESS = 0x80340022,
    /// <summary>The specified request is not a valid operation for the target device.</summary>
    ERROR_NDIS_INVALID_DEVICE_REQUEST = 0x80340010,
    /// <summary>The offload operation on the network interface has been paused.</summary>
    ERROR_NDIS_PAUSED = 0x8034002A,
    /// <summary>Network interface was not found.</summary>
    ERROR_NDIS_INTERFACE_NOT_FOUND = 0x8034002B,
    /// <summary>The revision number specified in the structure is not supported.</summary>
    ERROR_NDIS_UNSUPPORTED_REVISION = 0x8034002C,
    /// <summary>The specified port does not exist on this network interface.</summary>
    ERROR_NDIS_INVALID_PORT = 0x8034002D,
    /// <summary>The current state of the specified port on this network interface does not support the requested operation.</summary>
    ERROR_NDIS_INVALID_PORT_STATE = 0x8034002E,
    /// <summary>The miniport adapter is in low power state.</summary>
    ERROR_NDIS_LOW_POWER_STATE = 0x8034002F,
    /// <summary>The wireless local area network interface is in auto configuration mode and doesn&#39;t support the requested parameter change operation.</summary>
    ERROR_NDIS_DOT11_AUTO_CONFIG_ENABLED = 0x80342000,
    /// <summary>The wireless local area network interface is busy and can not perform the requested operation.</summary>
    ERROR_NDIS_DOT11_MEDIA_IN_USE = 0x80342001,
    /// <summary>The wireless local area network interface is power down and doesn&#39;t support the requested operation.</summary>
    ERROR_NDIS_DOT11_POWER_STATE_INVALID = 0x80342002,
    /// <summary>The list of wake on LAN patterns is full.</summary>
    ERROR_NDIS_PM_WOL_PATTERN_LIST_FULL = 0x80342003,
    /// <summary>The list of low power protocol offloads is full.</summary>
    ERROR_NDIS_PM_PROTOCOL_OFFLOAD_LIST_FULL = 0x80342004,
    /// <summary>The request will be completed later by NDIS status indication.</summary>
    ERROR_NDIS_INDICATION_REQUIRED = 0x00340001,
    /// <summary>The TCP connection is not offloadable because of a local policy setting.</summary>
    ERROR_NDIS_OFFLOAD_POLICY = 0xC034100F,
    /// <summary>The TCP connection is not offloadable by the Chimney Offload target.</summary>
    ERROR_NDIS_OFFLOAD_CONNECTION_REJECTED = 0xC0341012,
    /// <summary>The IP Path object is not in an offloadable state.</summary>
    ERROR_NDIS_OFFLOAD_PATH_REJECTED = 0xC0341013,
    /// <summary>The hypervisor does not support the operation because the specified hypercall code is not supported.</summary>
    ERROR_HV_INVALID_HYPERCALL_CODE = 0xC0350002,
    /// <summary>The hypervisor does not support the operation because the encoding for the hypercall input register is not supported.</summary>
    ERROR_HV_INVALID_HYPERCALL_INPUT = 0xC0350003,
    /// <summary>The hypervisor could not perform the operation beacuse a parameter has an invalid alignment.</summary>
    ERROR_HV_INVALID_ALIGNMENT = 0xC0350004,
    /// <summary>The hypervisor could not perform the operation beacuse an invalid parameter was specified.</summary>
    ERROR_HV_INVALID_PARAMETER = 0xC0350005,
    /// <summary>Access to the specified object was denied.</summary>
    ERROR_HV_ACCESS_DENIED = 0xC0350006,
    /// <summary>The hypervisor could not perform the operation because the partition is entering or in an invalid state.</summary>
    ERROR_HV_INVALID_PARTITION_STATE = 0xC0350007,
    /// <summary>The operation is not allowed in the current state.</summary>
    ERROR_HV_OPERATION_DENIED = 0xC0350008,
    /// <summary>The hypervisor does not recognize the specified partition property.</summary>
    ERROR_HV_UNKNOWN_PROPERTY = 0xC0350009,
    /// <summary>The specified value of a partition property is out of range or violates an invariant.</summary>
    ERROR_HV_PROPERTY_VALUE_OUT_OF_RANGE = 0xC035000A,
    /// <summary>There is not enough memory in the hypervisor pool to complete the operation.</summary>
    ERROR_HV_INSUFFICIENT_MEMORY = 0xC035000B,
    /// <summary>The maximum partition depth has been exceeded for the partition hierarchy.</summary>
    ERROR_HV_PARTITION_TOO_DEEP = 0xC035000C,
    /// <summary>A partition with the specified partition Id does not exist.</summary>
    ERROR_HV_INVALID_PARTITION_ID = 0xC035000D,
    /// <summary>The hypervisor could not perform the operation because the specified VP index is invalid.</summary>
    ERROR_HV_INVALID_VP_INDEX = 0xC035000E,
    /// <summary>The hypervisor could not perform the operation because the specified port identifier is invalid.</summary>
    ERROR_HV_INVALID_PORT_ID = 0xC0350011,
    /// <summary>The hypervisor could not perform the operation because the specified connection identifier is invalid.</summary>
    ERROR_HV_INVALID_CONNECTION_ID = 0xC0350012,
    /// <summary>Not enough buffers were supplied to send a message.</summary>
    ERROR_HV_INSUFFICIENT_BUFFERS = 0xC0350013,
    /// <summary>The previous virtual interrupt has not been acknowledged.</summary>
    ERROR_HV_NOT_ACKNOWLEDGED = 0xC0350014,
    /// <summary>The previous virtual interrupt has already been acknowledged.</summary>
    ERROR_HV_ACKNOWLEDGED = 0xC0350016,
    /// <summary>The indicated partition is not in a valid state for saving or restoring.</summary>
    ERROR_HV_INVALID_SAVE_RESTORE_STATE = 0xC0350017,
    /// <summary>The hypervisor could not complete the operation because a required feature of the synthetic interrupt controller (SynIC) was disabled.</summary>
    ERROR_HV_INVALID_SYNIC_STATE = 0xC0350018,
    /// <summary>The hypervisor could not perform the operation because the object or value was either already in use or being used for a purpose that would not permit completing the operation.</summary>
    ERROR_HV_OBJECT_IN_USE = 0xC0350019,
    /// <summary>The proximity domain information is invalid.</summary>
    ERROR_HV_INVALID_PROXIMITY_DOMAIN_INFO = 0xC035001A,
    /// <summary>An attempt to retrieve debugging data failed because none was available.</summary>
    ERROR_HV_NO_DATA = 0xC035001B,
    /// <summary>The physical connection being used for debuggging has not recorded any receive activity since the last operation.</summary>
    ERROR_HV_INACTIVE = 0xC035001C,
    /// <summary>There are not enough resources to complete the operation.</summary>
    ERROR_HV_NO_RESOURCES = 0xC035001D,
    /// <summary>A hypervisor feature is not available to the user.</summary>
    ERROR_HV_FEATURE_UNAVAILABLE = 0xC035001E,
    /// <summary>No hypervisor is present on this system.</summary>
    ERROR_HV_NOT_PRESENT = 0xC0351000,
    /// <summary>The handler for the virtualization infrastructure driver is already registered. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_DUPLICATE_HANDLER = 0xC0370001,
    /// <summary>The number of registered handlers for the virtualization infrastructure driver exceeded the maximum. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_TOO_MANY_HANDLERS = 0xC0370002,
    /// <summary>The message queue for the virtualization infrastructure driver is full and cannot accept new messages. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_QUEUE_FULL = 0xC0370003,
    /// <summary>No handler exists to handle the message for the virtualization infrastructure driver. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_HANDLER_NOT_PRESENT = 0xC0370004,
    /// <summary>The name of the partition or message queue for the virtualization infrastructure driver is invalid. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_INVALID_OBJECT_NAME = 0xC0370005,
    /// <summary>The partition name of the virtualization infrastructure driver exceeds the maximum.</summary>
    ERROR_VID_PARTITION_NAME_TOO_LONG = 0xC0370006,
    /// <summary>The message queue name of the virtualization infrastructure driver exceeds the maximum.</summary>
    ERROR_VID_MESSAGE_QUEUE_NAME_TOO_LONG = 0xC0370007,
    /// <summary>Cannot create the partition for the virtualization infrastructure driver because another partition with the same name already exists.</summary>
    ERROR_VID_PARTITION_ALREADY_EXISTS = 0xC0370008,
    /// <summary>The virtualization infrastructure driver has encountered an error. The requested partition does not exist. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_PARTITION_DOES_NOT_EXIST = 0xC0370009,
    /// <summary>The virtualization infrastructure driver has encountered an error. Could not find the requested partition. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_PARTITION_NAME_NOT_FOUND = 0xC037000A,
    /// <summary>A message queue with the same name already exists for the virtualization infrastructure driver.</summary>
    ERROR_VID_MESSAGE_QUEUE_ALREADY_EXISTS = 0xC037000B,
    /// <summary>The memory block page for the virtualization infrastructure driver cannot be mapped because the page map limit has been reached. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_EXCEEDED_MBP_ENTRY_MAP_LIMIT = 0xC037000C,
    /// <summary>The memory block for the virtualization infrastructure driver is still being used and cannot be destroyed.</summary>
    ERROR_VID_MB_STILL_REFERENCED = 0xC037000D,
    /// <summary>Cannot unlock the page array for the guest operating system memory address because it does not match a previous lock request. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_CHILD_GPA_PAGE_SET_CORRUPTED = 0xC037000E,
    /// <summary>The non-uniform memory access (NUMA) node settings do not match the system NUMA topology. In order to start the virtual machine, you will need to modify the NUMA configuration. For detailed information, see http://go.microsoft.com/fwlink/?LinkId=92362.</summary>
    ERROR_VID_INVALID_NUMA_SETTINGS = 0xC037000F,
    /// <summary>The non-uniform memory access (NUMA) node index does not match a valid index in the system NUMA topology.</summary>
    ERROR_VID_INVALID_NUMA_NODE_INDEX = 0xC0370010,
    /// <summary>The memory block for the virtualization infrastructure driver is already associated with a message queue.</summary>
    ERROR_VID_NOTIFICATION_QUEUE_ALREADY_ASSOCIATED = 0xC0370011,
    /// <summary>The handle is not a valid memory block handle for the virtualization infrastructure driver.</summary>
    ERROR_VID_INVALID_MEMORY_BLOCK_HANDLE = 0xC0370012,
    /// <summary>The request exceeded the memory block page limit for the virtualization infrastructure driver. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_PAGE_RANGE_OVERFLOW = 0xC0370013,
    /// <summary>The handle is not a valid message queue handle for the virtualization infrastructure driver.</summary>
    ERROR_VID_INVALID_MESSAGE_QUEUE_HANDLE = 0xC0370014,
    /// <summary>The handle is not a valid page range handle for the virtualization infrastructure driver.</summary>
    ERROR_VID_INVALID_GPA_RANGE_HANDLE = 0xC0370015,
    /// <summary>Cannot install client notifications because no message queue for the virtualization infrastructure driver is associated with the memory block.</summary>
    ERROR_VID_NO_MEMORY_BLOCK_NOTIFICATION_QUEUE = 0xC0370016,
    /// <summary>The request to lock or map a memory block page failed because the virtualization infrastructure driver memory block limit has been reached. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_MEMORY_BLOCK_LOCK_COUNT_EXCEEDED = 0xC0370017,
    /// <summary>The handle is not a valid parent partition mapping handle for the virtualization infrastructure driver.</summary>
    ERROR_VID_INVALID_PPM_HANDLE = 0xC0370018,
    /// <summary>Notifications cannot be created on the memory block because it is use.</summary>
    ERROR_VID_MBPS_ARE_LOCKED = 0xC0370019,
    /// <summary>The message queue for the virtualization infrastructure driver has been closed. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_MESSAGE_QUEUE_CLOSED = 0xC037001A,
    /// <summary>Cannot add a virtual processor to the partition because the maximum has been reached.</summary>
    ERROR_VID_VIRTUAL_PROCESSOR_LIMIT_EXCEEDED = 0xC037001B,
    /// <summary>Cannot stop the virtual processor immediately because of a pending intercept.</summary>
    ERROR_VID_STOP_PENDING = 0xC037001C,
    /// <summary>Invalid state for the virtual processor. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_INVALID_PROCESSOR_STATE = 0xC037001D,
    /// <summary>The maximum number of kernel mode clients for the virtualization infrastructure driver has been reached. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_EXCEEDED_KM_CONTEXT_COUNT_LIMIT = 0xC037001E,
    /// <summary>This kernel mode interface for the virtualization infrastructure driver has already been initialized. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_KM_INTERFACE_ALREADY_INITIALIZED = 0xC037001F,
    /// <summary>Cannot set or reset the memory block property more than once for the virtualization infrastructure driver. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_MB_PROPERTY_ALREADY_SET_RESET = 0xC0370020,
    /// <summary>The memory mapped I/O for this page range no longer exists. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_MMIO_RANGE_DESTROYED = 0xC0370021,
    /// <summary>The lock or unlock request uses an invalid guest operating system memory address. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_INVALID_CHILD_GPA_PAGE_SET = 0xC0370022,
    /// <summary>Cannot destroy or reuse the reserve page set for the virtualization infrastructure driver because it is in use. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_RESERVE_PAGE_SET_IS_BEING_USED = 0xC0370023,
    /// <summary>The reserve page set for the virtualization infrastructure driver is too small to use in the lock request. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_RESERVE_PAGE_SET_TOO_SMALL = 0xC0370024,
    /// <summary>Cannot lock or map the memory block page for the virtualization infrastructure driver because it has already been locked using a reserve page set page. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_MBP_ALREADY_LOCKED_USING_RESERVED_PAGE = 0xC0370025,
    /// <summary>Cannot create the memory block for the virtualization infrastructure driver because the requested number of pages exceeded the limit. Restarting the virtual machine may fix the problem. If the problem persists, try restarting the physical computer.</summary>
    ERROR_VID_MBP_COUNT_EXCEEDED_LIMIT = 0xC0370026,
    /// <summary>Cannot restore this virtual machine because the saved state data cannot be read. Delete the saved state data and then try to start the virtual machine.</summary>
    ERROR_VID_SAVED_STATE_CORRUPT = 0xC0370027,
    /// <summary>Cannot restore this virtual machine because an item read from the saved state data is not recognized. Delete the saved state data and then try to start the virtual machine.</summary>
    ERROR_VID_SAVED_STATE_UNRECOGNIZED_ITEM = 0xC0370028,
    /// <summary>Cannot restore this virtual machine to the saved state because of hypervisor incompatibility. Delete the saved state data and then try to start the virtual machine.</summary>
    ERROR_VID_SAVED_STATE_INCOMPATIBLE = 0xC0370029,
    /// <summary>A virtual machine is running with its memory allocated across multiple NUMA nodes. This does not indicate a problem unless the performance of your virtual machine is unusually slow. If you are experiencing performance problems, you may need to modify the NUMA configuration. For detailed information, see http://go.microsoft.com/fwlink/?LinkId=92362.</summary>
    ERROR_VID_REMOTE_NODE_PARENT_GPA_PAGES_USED = 0x80370001,
    /// <summary>The regeneration operation was not able to copy all data from the active plexes due to bad sectors.</summary>
    ERROR_VOLMGR_INCOMPLETE_REGENERATION = 0x80380001,
    /// <summary>One or more disks were not fully migrated to the target pack. They may or may not require reimport after fixing the hardware problems.</summary>
    ERROR_VOLMGR_INCOMPLETE_DISK_MIGRATION = 0x80380002,
    /// <summary>The configuration database is full.</summary>
    ERROR_VOLMGR_DATABASE_FULL = 0xC0380001,
    /// <summary>The configuration data on the disk is corrupted.</summary>
    ERROR_VOLMGR_DISK_CONFIGURATION_CORRUPTED = 0xC0380002,
    /// <summary>The configuration on the disk is not insync with the in-memory configuration.</summary>
    ERROR_VOLMGR_DISK_CONFIGURATION_NOT_IN_SYNC = 0xC0380003,
    /// <summary>A majority of disks failed to be updated with the new configuration.</summary>
    ERROR_VOLMGR_PACK_CONFIG_UPDATE_FAILED = 0xC0380004,
    /// <summary>The disk contains non-simple volumes.</summary>
    ERROR_VOLMGR_DISK_CONTAINS_NON_SIMPLE_VOLUME = 0xC0380005,
    /// <summary>The same disk was specified more than once in the migration list.</summary>
    ERROR_VOLMGR_DISK_DUPLICATE = 0xC0380006,
    /// <summary>The disk is already dynamic.</summary>
    ERROR_VOLMGR_DISK_DYNAMIC = 0xC0380007,
    /// <summary>The specified disk id is invalid. There are no disks with the specified disk id.</summary>
    ERROR_VOLMGR_DISK_ID_INVALID = 0xC0380008,
    /// <summary>The specified disk is an invalid disk. Operation cannot complete on an invalid disk.</summary>
    ERROR_VOLMGR_DISK_INVALID = 0xC0380009,
    /// <summary>The specified disk(s) cannot be removed since it is the last remaining voter.</summary>
    ERROR_VOLMGR_DISK_LAST_VOTER = 0xC038000A,
    /// <summary>The specified disk has an invalid disk layout.</summary>
    ERROR_VOLMGR_DISK_LAYOUT_INVALID = 0xC038000B,
    /// <summary>The disk layout contains non-basic partitions which appear after basic paritions. This is an invalid disk layout.</summary>
    ERROR_VOLMGR_DISK_LAYOUT_NON_BASIC_BETWEEN_BASIC_PARTITIONS = 0xC038000C,
    /// <summary>The disk layout contains partitions which are not cylinder aligned.</summary>
    ERROR_VOLMGR_DISK_LAYOUT_NOT_CYLINDER_ALIGNED = 0xC038000D,
    /// <summary>The disk layout contains partitions which are samller than the minimum size.</summary>
    ERROR_VOLMGR_DISK_LAYOUT_PARTITIONS_TOO_SMALL = 0xC038000E,
    /// <summary>The disk layout contains primary partitions in between logical drives. This is an invalid disk layout.</summary>
    ERROR_VOLMGR_DISK_LAYOUT_PRIMARY_BETWEEN_LOGICAL_PARTITIONS = 0xC038000F,
    /// <summary>The disk layout contains more than the maximum number of supported partitions.</summary>
    ERROR_VOLMGR_DISK_LAYOUT_TOO_MANY_PARTITIONS = 0xC0380010,
    /// <summary>The specified disk is missing. The operation cannot complete on a missing disk.</summary>
    ERROR_VOLMGR_DISK_MISSING = 0xC0380011,
    /// <summary>The specified disk is not empty.</summary>
    ERROR_VOLMGR_DISK_NOT_EMPTY = 0xC0380012,
    /// <summary>There is not enough usable space for this operation.</summary>
    ERROR_VOLMGR_DISK_NOT_ENOUGH_SPACE = 0xC0380013,
    /// <summary>The force revectoring of bad sectors failed.</summary>
    ERROR_VOLMGR_DISK_REVECTORING_FAILED = 0xC0380014,
    /// <summary>The specified disk has an invalid sector size.</summary>
    ERROR_VOLMGR_DISK_SECTOR_SIZE_INVALID = 0xC0380015,
    /// <summary>The specified disk set contains volumes which exist on disks outside of the set.</summary>
    ERROR_VOLMGR_DISK_SET_NOT_CONTAINED = 0xC0380016,
    /// <summary>A disk in the volume layout provides extents to more than one member of a plex.</summary>
    ERROR_VOLMGR_DISK_USED_BY_MULTIPLE_MEMBERS = 0xC0380017,
    /// <summary>A disk in the volume layout provides extents to more than one plex.</summary>
    ERROR_VOLMGR_DISK_USED_BY_MULTIPLE_PLEXES = 0xC0380018,
    /// <summary>Dynamic disks are not supported on this system.</summary>
    ERROR_VOLMGR_DYNAMIC_DISK_NOT_SUPPORTED = 0xC0380019,
    /// <summary>The specified extent is already used by other volumes.</summary>
    ERROR_VOLMGR_EXTENT_ALREADY_USED = 0xC038001A,
    /// <summary>The specified volume is retained and can only be extended into a contiguous extent. The specified extent to grow the volume is not contiguous with the specified volume.</summary>
    ERROR_VOLMGR_EXTENT_NOT_CONTIGUOUS = 0xC038001B,
    /// <summary>The specified volume extent is not within the public region of the disk.</summary>
    ERROR_VOLMGR_EXTENT_NOT_IN_PUBLIC_REGION = 0xC038001C,
    /// <summary>The specifed volume extent is not sector aligned.</summary>
    ERROR_VOLMGR_EXTENT_NOT_SECTOR_ALIGNED = 0xC038001D,
    /// <summary>The specified parition overlaps an EBR (the first track of an extended partition on a MBR disks).</summary>
    ERROR_VOLMGR_EXTENT_OVERLAPS_EBR_PARTITION = 0xC038001E,
    /// <summary>The specified extent lengths cannot be used to construct a volume with specified length.</summary>
    ERROR_VOLMGR_EXTENT_VOLUME_LENGTHS_DO_NOT_MATCH = 0xC038001F,
    /// <summary>The system does not support fault tolerant volumes.</summary>
    ERROR_VOLMGR_FAULT_TOLERANT_NOT_SUPPORTED = 0xC0380020,
    /// <summary>The specified interleave length is invalid.</summary>
    ERROR_VOLMGR_INTERLEAVE_LENGTH_INVALID = 0xC0380021,
    /// <summary>There is already a maximum number of registered users.</summary>
    ERROR_VOLMGR_MAXIMUM_REGISTERED_USERS = 0xC0380022,
    /// <summary>The specified member is already in-sync with the other active members. It does not need to be regenerated.</summary>
    ERROR_VOLMGR_MEMBER_IN_SYNC = 0xC0380023,
    /// <summary>The same member index was specified more than once.</summary>
    ERROR_VOLMGR_MEMBER_INDEX_DUPLICATE = 0xC0380024,
    /// <summary>The specified member index is greater or equal than the number of members in the volume plex.</summary>
    ERROR_VOLMGR_MEMBER_INDEX_INVALID = 0xC0380025,
    /// <summary>The specified member is missing. It cannot be regenerated.</summary>
    ERROR_VOLMGR_MEMBER_MISSING = 0xC0380026,
    /// <summary>The specified member is not detached. Cannot replace a member which is not detached.</summary>
    ERROR_VOLMGR_MEMBER_NOT_DETACHED = 0xC0380027,
    /// <summary>The specified member is already regenerating.</summary>
    ERROR_VOLMGR_MEMBER_REGENERATING = 0xC0380028,
    /// <summary>All disks belonging to the pack failed.</summary>
    ERROR_VOLMGR_ALL_DISKS_FAILED = 0xC0380029,
    /// <summary>There are currently no registered users for notifications. The task number is irrelevant unless there are registered users.</summary>
    ERROR_VOLMGR_NO_REGISTERED_USERS = 0xC038002A,
    /// <summary>The specified notification user does not exist. Failed to unregister user for notifications.</summary>
    ERROR_VOLMGR_NO_SUCH_USER = 0xC038002B,
    /// <summary>The notifications have been reset. Notifications for the current user are invalid. Unregister and re-register for notifications.</summary>
    ERROR_VOLMGR_NOTIFICATION_RESET = 0xC038002C,
    /// <summary>The specified number of members is invalid.</summary>
    ERROR_VOLMGR_NUMBER_OF_MEMBERS_INVALID = 0xC038002D,
    /// <summary>The specified number of plexes is invalid.</summary>
    ERROR_VOLMGR_NUMBER_OF_PLEXES_INVALID = 0xC038002E,
    /// <summary>The specified source and target packs are identical.</summary>
    ERROR_VOLMGR_PACK_DUPLICATE = 0xC038002F,
    /// <summary>The specified pack id is invalid. There are no packs with the specified pack id.</summary>
    ERROR_VOLMGR_PACK_ID_INVALID = 0xC0380030,
    /// <summary>The specified pack is the invalid pack. The operation cannot complete with the invalid pack.</summary>
    ERROR_VOLMGR_PACK_INVALID = 0xC0380031,
    /// <summary>The specified pack name is invalid.</summary>
    ERROR_VOLMGR_PACK_NAME_INVALID = 0xC0380032,
    /// <summary>The specified pack is offline.</summary>
    ERROR_VOLMGR_PACK_OFFLINE = 0xC0380033,
    /// <summary>The specified pack already has a quorum of healthy disks.</summary>
    ERROR_VOLMGR_PACK_HAS_QUORUM = 0xC0380034,
    /// <summary>The pack does not have a quorum of healthy disks.</summary>
    ERROR_VOLMGR_PACK_WITHOUT_QUORUM = 0xC0380035,
    /// <summary>The specified disk has an unsupported partition style. Only MBR and GPT partition styles are supported.</summary>
    ERROR_VOLMGR_PARTITION_STYLE_INVALID = 0xC0380036,
    /// <summary>Failed to update the disk&#39;s partition layout.</summary>
    ERROR_VOLMGR_PARTITION_UPDATE_FAILED = 0xC0380037,
    /// <summary>The specified plex is already in-sync with the other active plexes. It does not need to be regenerated.</summary>
    ERROR_VOLMGR_PLEX_IN_SYNC = 0xC0380038,
    /// <summary>The same plex index was specified more than once.</summary>
    ERROR_VOLMGR_PLEX_INDEX_DUPLICATE = 0xC0380039,
    /// <summary>The specified plex index is greater or equal than the number of plexes in the volume.</summary>
    ERROR_VOLMGR_PLEX_INDEX_INVALID = 0xC038003A,
    /// <summary>The specified plex is the last active plex in the volume. The plex cannot be removed or else the volume will go offline.</summary>
    ERROR_VOLMGR_PLEX_LAST_ACTIVE = 0xC038003B,
    /// <summary>The specified plex is missing.</summary>
    ERROR_VOLMGR_PLEX_MISSING = 0xC038003C,
    /// <summary>The specified plex is currently regenerating.</summary>
    ERROR_VOLMGR_PLEX_REGENERATING = 0xC038003D,
    /// <summary>The specified plex type is invalid.</summary>
    ERROR_VOLMGR_PLEX_TYPE_INVALID = 0xC038003E,
    /// <summary>The operation is only supported on RAID-5 plexes.</summary>
    ERROR_VOLMGR_PLEX_NOT_RAID5 = 0xC038003F,
    /// <summary>The operation is only supported on simple plexes.</summary>
    ERROR_VOLMGR_PLEX_NOT_SIMPLE = 0xC0380040,
    /// <summary>The Size fields in the VM_VOLUME_LAYOUT input structure are incorrectly set.</summary>
    ERROR_VOLMGR_STRUCTURE_SIZE_INVALID = 0xC0380041,
    /// <summary>There is already a pending request for notifications. Wait for the existing request to return before requesting for more notifications.</summary>
    ERROR_VOLMGR_TOO_MANY_NOTIFICATION_REQUESTS = 0xC0380042,
    /// <summary>There is currently a transaction in process.</summary>
    ERROR_VOLMGR_TRANSACTION_IN_PROGRESS = 0xC0380043,
    /// <summary>An unexpected layout change occurred outside of the volume manager.</summary>
    ERROR_VOLMGR_UNEXPECTED_DISK_LAYOUT_CHANGE = 0xC0380044,
    /// <summary>The specified volume contains a missing disk.</summary>
    ERROR_VOLMGR_VOLUME_CONTAINS_MISSING_DISK = 0xC0380045,
    /// <summary>The specified volume id is invalid. There are no volumes with the specified volume id.</summary>
    ERROR_VOLMGR_VOLUME_ID_INVALID = 0xC0380046,
    /// <summary>The specified volume length is invalid.</summary>
    ERROR_VOLMGR_VOLUME_LENGTH_INVALID = 0xC0380047,
    /// <summary>The specified size for the volume is not a multiple of the sector size.</summary>
    ERROR_VOLMGR_VOLUME_LENGTH_NOT_SECTOR_SIZE_MULTIPLE = 0xC0380048,
    /// <summary>The operation is only supported on mirrored volumes.</summary>
    ERROR_VOLMGR_VOLUME_NOT_MIRRORED = 0xC0380049,
    /// <summary>The specified volume does not have a retain partition.</summary>
    ERROR_VOLMGR_VOLUME_NOT_RETAINED = 0xC038004A,
    /// <summary>The specified volume is offline.</summary>
    ERROR_VOLMGR_VOLUME_OFFLINE = 0xC038004B,
    /// <summary>The specified volume already has a retain partition.</summary>
    ERROR_VOLMGR_VOLUME_RETAINED = 0xC038004C,
    /// <summary>The specified number of extents is invalid.</summary>
    ERROR_VOLMGR_NUMBER_OF_EXTENTS_INVALID = 0xC038004D,
    /// <summary>All disks participating to the volume must have the same sector size.</summary>
    ERROR_VOLMGR_DIFFERENT_SECTOR_SIZE = 0xC038004E,
    /// <summary>The boot disk experienced failures.</summary>
    ERROR_VOLMGR_BAD_BOOT_DISK = 0xC038004F,
    /// <summary>The configuration of the pack is offline.</summary>
    ERROR_VOLMGR_PACK_CONFIG_OFFLINE = 0xC0380050,
    /// <summary>The configuration of the pack is online.</summary>
    ERROR_VOLMGR_PACK_CONFIG_ONLINE = 0xC0380051,
    /// <summary>The specified pack is not the primary pack.</summary>
    ERROR_VOLMGR_NOT_PRIMARY_PACK = 0xC0380052,
    /// <summary>All disks failed to be updated with the new content of the log.</summary>
    ERROR_VOLMGR_PACK_LOG_UPDATE_FAILED = 0xC0380053,
    /// <summary>The specified number of disks in a plex is invalid.</summary>
    ERROR_VOLMGR_NUMBER_OF_DISKS_IN_PLEX_INVALID = 0xC0380054,
    /// <summary>The specified number of disks in a plex member is invalid.</summary>
    ERROR_VOLMGR_NUMBER_OF_DISKS_IN_MEMBER_INVALID = 0xC0380055,
    /// <summary>The operation is not supported on mirrored volumes.</summary>
    ERROR_VOLMGR_VOLUME_MIRRORED = 0xC0380056,
    /// <summary>The operation is only supported on simple and spanned plexes.</summary>
    ERROR_VOLMGR_PLEX_NOT_SIMPLE_SPANNED = 0xC0380057,
    /// <summary>The pack has no valid log copies.</summary>
    ERROR_VOLMGR_NO_VALID_LOG_COPIES = 0xC0380058,
    /// <summary>A primary pack is already present.</summary>
    ERROR_VOLMGR_PRIMARY_PACK_PRESENT = 0xC0380059,
    /// <summary>The specified number of disks is invalid.</summary>
    ERROR_VOLMGR_NUMBER_OF_DISKS_INVALID = 0xC038005A,
    /// <summary>The system does not support mirrored volumes.</summary>
    ERROR_VOLMGR_MIRROR_NOT_SUPPORTED = 0xC038005B,
    /// <summary>The system does not support RAID-5 volumes.</summary>
    ERROR_VOLMGR_RAID5_NOT_SUPPORTED = 0xC038005C,
    /// <summary>Some BCD entries were not imported correctly from the BCD store.</summary>
    ERROR_BCD_NOT_ALL_ENTRIES_IMPORTED = 0x80390001,
    /// <summary>Entries enumerated have exceeded the allowed threshold.</summary>
    ERROR_BCD_TOO_MANY_ELEMENTS = 0xC0390002,
    /// <summary>Some BCD entries were not synchronized correctly with the firmware.</summary>
    ERROR_BCD_NOT_ALL_ENTRIES_SYNCHRONIZED = 0x80390003,
    /// <summary>The virtual hard disk is corrupted. The virtual hard disk drive footer is missing.</summary>
    ERROR_VHD_DRIVE_FOOTER_MISSING = 0xC03A0001,
    /// <summary>The virtual hard disk is corrupted. The virtual hard disk drive footer checksum does not match the on-disk checksum.</summary>
    ERROR_VHD_DRIVE_FOOTER_CHECKSUM_MISMATCH = 0xC03A0002,
    /// <summary>The virtual hard disk is corrupted. The virtual hard disk drive footer in the virtual hard disk is corrupted.</summary>
    ERROR_VHD_DRIVE_FOOTER_CORRUPT = 0xC03A0003,
    /// <summary>The system does not recognize the file format of this virtual hard disk.</summary>
    ERROR_VHD_FORMAT_UNKNOWN = 0xC03A0004,
    /// <summary>The version does not support this version of the file format.</summary>
    ERROR_VHD_FORMAT_UNSUPPORTED_VERSION = 0xC03A0005,
    /// <summary>The virtual hard disk is corrupted. The sparse header checksum does not match the on-disk checksum.</summary>
    ERROR_VHD_SPARSE_HEADER_CHECKSUM_MISMATCH = 0xC03A0006,
    /// <summary>The system does not support this version of the virtual hard disk.This version of the sparse header is not supported.</summary>
    ERROR_VHD_SPARSE_HEADER_UNSUPPORTED_VERSION = 0xC03A0007,
    /// <summary>The virtual hard disk is corrupted. The sparse header in the virtual hard disk is corrupt.</summary>
    ERROR_VHD_SPARSE_HEADER_CORRUPT = 0xC03A0008,
    /// <summary>Failed to write to the virtual hard disk failed because the system failed to allocate a new block in the virtual hard disk.</summary>
    ERROR_VHD_BLOCK_ALLOCATION_FAILURE = 0xC03A0009,
    /// <summary>The virtual hard disk is corrupted. The block allocation table in the virtual hard disk is corrupt.</summary>
    ERROR_VHD_BLOCK_ALLOCATION_TABLE_CORRUPT = 0xC03A000A,
    /// <summary>The system does not support this version of the virtual hard disk. The block size is invalid.</summary>
    ERROR_VHD_INVALID_BLOCK_SIZE = 0xC03A000B,
    /// <summary>The virtual hard disk is corrupted. The block bitmap does not match with the block data present in the virtual hard disk.</summary>
    ERROR_VHD_BITMAP_MISMATCH = 0xC03A000C,
    /// <summary>The chain of virtual hard disks is broken. The system cannot locate the parent virtual hard disk for the differencing disk.</summary>
    ERROR_VHD_PARENT_VHD_NOT_FOUND = 0xC03A000D,
    /// <summary>The chain of virtual hard disks is corrupted. There is a mismatch in the identifiers of the parent virtual hard disk and differencing disk.</summary>
    ERROR_VHD_CHILD_PARENT_ID_MISMATCH = 0xC03A000E,
    /// <summary>The chain of virtual hard disks is corrupted. The time stamp of the parent virtual hard disk does not match the time stamp of the differencing disk.</summary>
    ERROR_VHD_CHILD_PARENT_TIMESTAMP_MISMATCH = 0xC03A000F,
    /// <summary>Failed to read the metadata of the virtual hard disk.</summary>
    ERROR_VHD_METADATA_READ_FAILURE = 0xC03A0010,
    /// <summary>Failed to write to the metadata of the virtual hard disk.</summary>
    ERROR_VHD_METADATA_WRITE_FAILURE = 0xC03A0011,
    /// <summary>The size of the virtual hard disk is not valid.</summary>
    ERROR_VHD_INVALID_SIZE = 0xC03A0012,
    /// <summary>The file size of this virtual hard disk is not valid.</summary>
    ERROR_VHD_INVALID_FILE_SIZE = 0xC03A0013,
    /// <summary>A virtual disk support provider for the specified file was not found.</summary>
    ERROR_VIRTDISK_PROVIDER_NOT_FOUND = 0xC03A0014,
    /// <summary>The specified disk is not a virtual disk.</summary>
    ERROR_VIRTDISK_NOT_VIRTUAL_DISK = 0xC03A0015,
    /// <summary>The chain of virtual hard disks is inaccessible. The process has not been granted access rights to the parent virtual hard disk for the differencing disk.</summary>
    ERROR_VHD_PARENT_VHD_ACCESS_DENIED = 0xC03A0016,
    /// <summary>The chain of virtual hard disks is corrupted. There is a mismatch in the virtual sizes of the parent virtual hard disk and differencing disk.</summary>
    ERROR_VHD_CHILD_PARENT_SIZE_MISMATCH = 0xC03A0017,
    /// <summary>The chain of virtual hard disks is corrupted. A differencing disk is indicated in its own parent chain.</summary>
    ERROR_VHD_DIFFERENCING_CHAIN_CYCLE_DETECTED = 0xC03A0018,
    /// <summary>The chain of virtual hard disks is inaccessible. There was an error opening a virtual hard disk further up the chain.</summary>
    ERROR_VHD_DIFFERENCING_CHAIN_ERROR_IN_PARENT = 0xC03A0019,
    /// <summary>The requested operation could not be completed due to a virtual disk system limitation.  Virtual disks are only supported on NTFS volumes and must be both uncompressed and unencrypted.</summary>
    ERROR_VIRTUAL_DISK_LIMITATION = 0xC03A001A,
    /// <summary>The requested operation cannot be performed on a virtual disk of this type.</summary>
    ERROR_VHD_INVALID_TYPE = 0xC03A001B,
    /// <summary>The requested operation cannot be performed on the virtual disk in its current state.</summary>
    ERROR_VHD_INVALID_STATE = 0xC03A001C,
    /// <summary>The sector size of the physical disk on which the virtual disk resides is not supported.</summary>
    ERROR_VIRTDISK_UNSUPPORTED_DISK_SECTOR_SIZE = 0xC03A001D,
    /// <summary>The virtualization storage subsystem has generated an error.</summary>
    ERROR_QUERY_STORAGE_ERROR = 0x803A0001,
    /// <summary>The operation was cancelled.</summary>
    SDIAG_E_CANCELLED = 0x803C0100,
    /// <summary>An error occurred when running a PowerShell script.</summary>
    SDIAG_E_SCRIPT = 0x803C0101,
    /// <summary>An error occurred when interacting with PowerShell runtime.</summary>
    SDIAG_E_POWERSHELL = 0x803C0102,
    /// <summary>An error occurred in the Scripted Diagnostic Managed Host.</summary>
    SDIAG_E_MANAGEDHOST = 0x803C0103,
    /// <summary>The troubleshooting pack does not contain a required verifier to complete the verification.</summary>
    SDIAG_E_NOVERIFIER = 0x803C0104,
    /// <summary>The troubleshooting pack cannot be executed on this system.</summary>
    SDIAG_S_CANNOTRUN = 0x003C0105,
    /// <summary>Scripted diagnostics is disabled by group policy.</summary>
    SDIAG_E_DISABLED = 0x803C0106,
    /// <summary>Trust validation of the troubleshooting pack failed.</summary>
    SDIAG_E_TRUST = 0x803C0107,
    /// <summary>The troubleshooting pack cannot be executed on this system.</summary>
    SDIAG_E_CANNOTRUN = 0x803C0108,
    /// <summary>This version of the troubleshooting pack is not supported.</summary>
    SDIAG_E_VERSION = 0x803C0109,
    /// <summary>A required resource cannot be loaded.</summary>
    SDIAG_E_RESOURCE = 0x803C010A,
    /// <summary>The troubleshooting pack reported information for a root cause without adding the root cause.</summary>
    SDIAG_E_ROOTCAUSE = 0x803C010B,
    /// <summary>Context is not activated.</summary>
    E_MBN_CONTEXT_NOT_ACTIVATED = 0x80548201,
    /// <summary>Bad SIM is inserted.</summary>
    E_MBN_BAD_SIM = 0x80548202,
    /// <summary>Requested data class is not avaialable.</summary>
    E_MBN_DATA_CLASS_NOT_AVAILABLE = 0x80548203,
    /// <summary>Access point name (APN) or Access string is incorrect.</summary>
    E_MBN_INVALID_ACCESS_STRING = 0x80548204,
    /// <summary>Max activated contexts have reached.</summary>
    E_MBN_MAX_ACTIVATED_CONTEXTS = 0x80548205,
    /// <summary>Device is in packet detach state.</summary>
    E_MBN_PACKET_SVC_DETACHED = 0x80548206,
    /// <summary>Provider is not visible.</summary>
    E_MBN_PROVIDER_NOT_VISIBLE = 0x80548207,
    /// <summary>Radio is powered off.</summary>
    E_MBN_RADIO_POWER_OFF = 0x80548208,
    /// <summary>MBN subscription is not activated.</summary>
    E_MBN_SERVICE_NOT_ACTIVATED = 0x80548209,
    /// <summary>SIM is not inserted.</summary>
    E_MBN_SIM_NOT_INSERTED = 0x8054820A,
    /// <summary>Voice call in progress.</summary>
    E_MBN_VOICE_CALL_IN_PROGRESS = 0x8054820B,
    /// <summary>Visible provider cache is invalid.</summary>
    E_MBN_INVALID_CACHE = 0x8054820C,
    /// <summary>Device is not registered.</summary>
    E_MBN_NOT_REGISTERED = 0x8054820D,
    /// <summary>Providers not found.</summary>
    E_MBN_PROVIDERS_NOT_FOUND = 0x8054820E,
    /// <summary>Pin is not supported.</summary>
    E_MBN_PIN_NOT_SUPPORTED = 0x8054820F,
    /// <summary>Pin is required.</summary>
    E_MBN_PIN_REQUIRED = 0x80548210,
    /// <summary>PIN is disabled.</summary>
    E_MBN_PIN_DISABLED = 0x80548211,
    /// <summary>Generic Failure.</summary>
    E_MBN_FAILURE = 0x80548212,
    /// <summary>Profile is invalid.</summary>
    E_MBN_INVALID_PROFILE = 0x80548218,
    /// <summary>Default profile exist.</summary>
    E_MBN_DEFAULT_PROFILE_EXIST = 0x80548219,
    /// <summary>SMS encoding is not supported.</summary>
    E_MBN_SMS_ENCODING_NOT_SUPPORTED = 0x80548220,
    /// <summary>SMS filter is not supported.</summary>
    E_MBN_SMS_FILTER_NOT_SUPPORTED = 0x80548221,
    /// <summary>Invalid SMS memory index is used.</summary>
    E_MBN_SMS_INVALID_MEMORY_INDEX = 0x80548222,
    /// <summary>SMS language is not supported.</summary>
    E_MBN_SMS_LANG_NOT_SUPPORTED = 0x80548223,
    /// <summary>SMS memory failure occurred.</summary>
    E_MBN_SMS_MEMORY_FAILURE = 0x80548224,
    /// <summary>SMS network timeout happened.</summary>
    E_MBN_SMS_NETWORK_TIMEOUT = 0x80548225,
    /// <summary>Unknown SMSC address is used.</summary>
    E_MBN_SMS_UNKNOWN_SMSC_ADDRESS = 0x80548226,
    /// <summary>SMS format is not supported.</summary>
    E_MBN_SMS_FORMAT_NOT_SUPPORTED = 0x80548227,
    /// <summary>SMS operation is not allowed.</summary>
    E_MBN_SMS_OPERATION_NOT_ALLOWED = 0x80548228,
    /// <summary>Device SMS memory is full.</summary>
    E_MBN_SMS_MEMORY_FULL = 0x80548229,
    /// <summary>The object could not be created.</summary>
    UI_E_CREATE_FAILED = 0x802A0001,
    /// <summary>Shutdown was already called on this object or the object that owns it.</summary>
    UI_E_SHUTDOWN_CALLED = 0x802A0002,
    /// <summary>This method cannot be called during this type of callback.</summary>
    UI_E_ILLEGAL_REENTRANCY = 0x802A0003,
    /// <summary>This object has been sealed, so this change is no longer allowed.</summary>
    UI_E_OBJECT_SEALED = 0x802A0004,
    /// <summary>The requested value was never set.</summary>
    UI_E_VALUE_NOT_SET = 0x802A0005,
    /// <summary>The requested value cannot be determined.</summary>
    UI_E_VALUE_NOT_DETERMINED = 0x802A0006,
    /// <summary>A callback returned an invalid output parameter.</summary>
    UI_E_INVALID_OUTPUT = 0x802A0007,
    /// <summary>A callback returned a success code other than S_OK or S_FALSE.</summary>
    UI_E_BOOLEAN_EXPECTED = 0x802A0008,
    /// <summary>A parameter that should be owned by this object is owned by a different object.</summary>
    UI_E_DIFFERENT_OWNER = 0x802A0009,
    /// <summary>More than one item matched the search criteria.</summary>
    UI_E_AMBIGUOUS_MATCH = 0x802A000A,
    /// <summary>A floating-point overflow occurred.</summary>
    UI_E_FP_OVERFLOW = 0x802A000B,
    /// <summary>This method can only be called from the thread that created the object.</summary>
    UI_E_WRONG_THREAD = 0x802A000C,
    /// <summary>The storyboard is currently in the schedule.</summary>
    UI_E_STORYBOARD_ACTIVE = 0x802A0101,
    /// <summary>The storyboard is not playing.</summary>
    UI_E_STORYBOARD_NOT_PLAYING = 0x802A0102,
    /// <summary>The start keyframe might occur after the end keyframe.</summary>
    UI_E_START_KEYFRAME_AFTER_END = 0x802A0103,
    /// <summary>It might not be possible to determine the end keyframe time when the start keyframe is reached.</summary>
    UI_E_END_KEYFRAME_NOT_DETERMINED = 0x802A0104,
    /// <summary>Two repeated portions of a storyboard might overlap.</summary>
    UI_E_LOOPS_OVERLAP = 0x802A0105,
    /// <summary>The transition has already been added to a storyboard.</summary>
    UI_E_TRANSITION_ALREADY_USED = 0x802A0106,
    /// <summary>The transition has not been added to a storyboard.</summary>
    UI_E_TRANSITION_NOT_IN_STORYBOARD = 0x802A0107,
    /// <summary>The transition might eclipse the beginning of another transition in the storyboard.</summary>
    UI_E_TRANSITION_ECLIPSED = 0x802A0108,
    /// <summary>The given time is earlier than the time passed to the last update.</summary>
    UI_E_TIME_BEFORE_LAST_UPDATE = 0x802A0109,
    /// <summary>This client is already connected to a timer.</summary>
    UI_E_TIMER_CLIENT_ALREADY_CONNECTED = 0x802A010A,
}

public static class ErreurWindows
{
    public static string RetourneTexteErreur(int numErreur)
    {
        uint err = Convert.ToUInt32("0x" + numErreur.ToString("X"), 16);
        Win32Error win32Error;
        win32Error = (Win32Error)err;
        return win32Error.ToString("G");
    }
}
