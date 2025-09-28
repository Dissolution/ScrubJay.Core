namespace ScrubJay.Extensions;

[PublicAPI]
public static class ListExtensions
{
    extension<T>(List<T>? list)
    {
        public void ForEach(ActRef<T> perItem)
        {
            if (list is not null)
            {
                T temp;

                for (int i = 0; i < list.Count; i++)
                {
                    temp = list[i];
                    perItem(ref temp);
                    list[i] = temp;
                }
            }
        }
    }
}