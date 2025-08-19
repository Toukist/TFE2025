export interface Privilege {
  id: number;
  name: string;
  description: string;
  isConfigurableRead: boolean;
  isConfigurableDelete: boolean;
  isConfigurableAdd: boolean;
  isConfigurableModify: boolean;
  isConfigurableStatus: boolean;
  isConfigurableExecution: boolean;
  lastEditBy: string;
  lastEditAt: Date;
}
