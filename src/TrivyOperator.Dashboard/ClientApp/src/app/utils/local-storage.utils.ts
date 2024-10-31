export class LocalStorageUtils {
  public static readonly csvFileNameKeyPrefix: string = "csvFileName.";
  public static readonly trivyTableKeyPrefix: string = "trivyTable.";

  public static getKeysWithPrefix(prefix: string): string[] {
    const keys: string[] = [];
    for (let i = 0; i < localStorage.length; i++) {
      const key = localStorage.key(i);
      if (key && key.startsWith(prefix)) {
        keys.push(key);
      }
    }
    return keys;
  }
}
