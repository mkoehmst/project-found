namespace ProjectFound.Misc
{


	using System;
	using System.Collections.Concurrent;

	public class GenericPool<T>
	{
		private ConcurrentBag<T> _objects;
		private Func<T> _objectGenerator;

		public GenericPool( Func<T> objectGenerator, int initialCount = 8 )
		{
			if ( objectGenerator == null ) throw new ArgumentNullException( "objectGenerator" );
			_objects = new ConcurrentBag<T>( );
			_objectGenerator = objectGenerator;

			// 8 seems like a reasonable minimum count
			Generate( initialCount < 8 ? 8 : initialCount );
		}

		public void Claim( out T assignment )
		{
			if ( _objects.TryTake( out assignment ) == false )
			{
				Grow( );
				Claim( out assignment );
			}
		}

		public void Rescind( ref T rescindee )
		{
			_objects.Add( rescindee );
		}

		private void Generate( int count )
		{
			for ( int i = 0; i < count; ++i )
				_objects.Add( _objectGenerator( ) );
		}

		private void Grow( )
		{
			int currentCount = _objects.Count;

			if ( currentCount < 2 )
			{
				Generate( 16 );
			}
			else
			{
				// Grow half-again as large each time
				// Example: 64 + (32) = 96
				int updatedCount = currentCount + (currentCount >> 1);

				Generate( updatedCount );
			}
		}
	}


}
