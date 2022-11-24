export interface IUserLoginRquest {
    userName: string;
};

export interface IUserLoginResponse {
    userId: number;
    userChatInfos: UserChatInfo[];
};

export interface UserChatInfo {
    chatId: number;
    chatName: string;
}